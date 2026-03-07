using Application.Commands.Movie;
using Application.DTOs.Request.Movie;
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
        private readonly AddAwardsToMovieCommand _validCommand;

        #region CONSTRUCTOR
        public AddUseCaseTests()
        {
            _subMovieRepository = Substitute.For<IMovieRepository>();
            _subUnitOfWork = Substitute.For<IUnitOfWork>();

            _sut = new AddAwardUseCase(_subMovieRepository, _subUnitOfWork);

            _validMovie = TestDataFactory.CreateInceptionMovie().Success!;

            var awardItems = new List<AwardItemRequest>
            {
                new AwardItemRequest(
                    AwardCategory.BestCinematography.Id,
                    Institution.BAFTA.Id,
                    2011)
            };

            _validCommand = new AddAwardsToMovieCommand(_validMovie.Id, awardItems);

            SetupRepositoryReturn(_validMovie);
        }
        #endregion

        #region Helpers
        private void SetupRepositoryReturn(Movie? movie)
        {
            _subMovieRepository.GetByIdWithAwardAsync(Arg.Any<Guid>())
                               .Returns(Task.FromResult(movie));
        }
        #endregion

        #region HAPPY PATH
        [Fact]
        public async Task Handle_WhenAllDataIsValid_ShouldReturnSuccessAndCommit()
        {
            // Act
            var result = await _sut.Handle(_validCommand, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
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
            SetupRepositoryReturn(null);

            // Act
            var result = await _sut.Handle(_validCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Failure!.Type.Should().Be(FailureType.NotFound);
            await _subUnitOfWork.DidNotReceive().Commit(Arg.Any<CancellationToken>());
        }

        [Theory]
        [InlineData(9999, 1)] // Categoria inválida
        [InlineData(1, 9999)] // Instituição inválida
        public async Task Handle_WhenSmartEnumIdIsInvalid_ShouldReturnValidationFailure(int categoryId, int institutionId)
        {
            // Arrange
            var badItems = new List<AwardItemRequest> { new AwardItemRequest(categoryId, institutionId, 2011) };
            var badCommand = new AddAwardsToMovieCommand(_validMovie.Id, badItems);

            // Act
            var result = await _sut.Handle(badCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Failure!.Type.Should().Be(FailureType.Validation);
        }

        [Fact]
        public async Task Handle_WhenAwardCreationFails_ShouldReturnValidationFailure()
        {
            var badItems = new List<AwardItemRequest> { new AwardItemRequest(1, 1, 1800) };
            var badCommand = new AddAwardsToMovieCommand(_validMovie.Id, badItems);

            // Act
            var result = await _sut.Handle(badCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Failure!.Type.Should().Be(FailureType.Validation);
        }

        [Fact]
        public async Task Handle_WhenMovieAddAwardFails_ShouldReturnConflictFailure()
        {
            var duplicateItems = new List<AwardItemRequest>
            {
                new AwardItemRequest(
                    AwardCategory.BestOriginalScreenplay.Id,
                    Institution.AcademyAwards.Id,
                    2011)
            };
            var duplicateCommand = new AddAwardsToMovieCommand(_validMovie.Id, duplicateItems);

            // Act
            var result = await _sut.Handle(duplicateCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Failure!.Type.Should().Be(FailureType.Conflict);
            result.Failure.Message.Should().Contain("já possui");
        }
        #endregion
    }
}
