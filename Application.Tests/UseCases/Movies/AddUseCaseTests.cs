using Application.Commands.Movie;
using Application.UseCases.Movies;
using Domain.Entities;
using Domain.Enums;
using Domain.SeedWork.Interfaces;
using Domain.SmartEnums;
using FluentAssertions;
using NSubstitute;
using Tests.Shared;

namespace Application.Tests.UseCases.Movies
{
    public class AddUseCaseTests
    {
        private readonly AddAwardUseCase _sut;
        private readonly IMovieRepository _subMovieRepository;
        private readonly IUnitOfWork _subUnitOfWork;

        private readonly Movie _validMovie;
        private readonly AddAwardCommand _validCommand;

        #region CONSTRUCTOR
        public AddUseCaseTests()
        {
            _subMovieRepository = Substitute.For<IMovieRepository>();
            _subUnitOfWork = Substitute.For<IUnitOfWork>();

            _sut = new AddAwardUseCase(
                _subMovieRepository,
                _subUnitOfWork);

            _validMovie = TestDataFactory.CreateInceptionMovie().Success!;

            _validCommand = new AddAwardCommand(
                _validMovie.Id,
                AwardCategory.BestCinematography.Id,
                Institution.BAFTA.Id,           
                2011 
            );

            _subMovieRepository.GetByIdWithAwardAsync(_validCommand.Id)
                               .Returns(Task.FromResult<Movie?>(_validMovie));
        }
        #endregion

        #region HAPPY PATH
        [Fact]
        public async Task Handle_WhenAllDataIsValid_ShouldReturnSuccessAndCommit()
        {
            // Act
            var result = await _sut.Handle(_validCommand, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().Be(true);
            result.Success.Should().Be(true);

            _validMovie.Awards.Should().HaveCount(2);
            _validMovie.Awards.Should().Contain(a => a.Category == AwardCategory.BestCinematography);

            await _subUnitOfWork.Received(1).Commit(Arg.Any<CancellationToken>());
        }
        #endregion

        #region UNHappy PATH
        [Fact]
        public async Task Handle_WhenMovieIsNotFound_ShouldReturnNotFoundFailure()
        {
            // Arrange
            _subMovieRepository.GetByIdWithAwardAsync(_validCommand.Id)
                               .Returns(Task.FromResult<Movie?>(null));

            // Act
            var result = await _sut.Handle(_validCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().Be(true);
            result.Failure.Type.Should().Be(FailureType.NotFound);
            result.Failure.Message.Should().Contain("Filme");

            await _subUnitOfWork.DidNotReceive().Commit(Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData(9999, 1)]
        [InlineData(1, 9999)]
        public async Task Handle_WhenSmartEnumIdIsInvalid_ShouldReturnValidationFailure(int categoryId, int institutionId)
        {
            // Arrange
            var badCommand = new AddAwardCommand(
                _validMovie.Id,
                categoryId,
                institutionId,
                2011
            );

            _subMovieRepository.GetByIdWithAwardAsync(_validMovie.Id)
                               .Returns(Task.FromResult<Movie?>(_validMovie));

            // Act
            var result = await _sut.Handle(badCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().Be(true);
            result.Failure.Type.Should().Be(FailureType.Validation);
            result.Failure.Message.Should().Contain("Smart Enum");
            await _subUnitOfWork.DidNotReceive().Commit(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenAwardCreationFails_ShouldReturnValidationFailure()
        {
            // Arrange
            var badCommand = _validCommand with { Year = 1800 };

            // Act
            var result = await _sut.Handle(badCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().Be(true);
            result.Failure.Type.Should().Be(FailureType.Validation); 
            await _subUnitOfWork.DidNotReceive().Commit(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task Handle_WhenMovieAddAwardFails_ShouldReturnDomainFailure()
        {
            // Arrange
            var duplicateAwardCommand = new AddAwardCommand(
                _validMovie.Id,
                AwardCategory.BestOriginalScreenplay.Id, 
                Institution.AcademyAwards.Id,           
                2011
            );

            // Act
            var result = await _sut.Handle(duplicateAwardCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().Be(true);
            result.Failure.Type.Should().Be(FailureType.Conflict);
            result.Failure.Message.Should().Contain("Filme já possui este prêmio neste ano"); // ou similar
            await _subUnitOfWork.DidNotReceive().Commit(Arg.Any<CancellationToken>());
        }
        #endregion
    }
}
