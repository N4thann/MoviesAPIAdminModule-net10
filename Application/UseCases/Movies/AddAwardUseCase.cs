
using Application.Commands.Movie;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Domain.SmartEnums;
using Domain.ValueObjects;

namespace Application.UseCases.Movies
{
    public class AddAwardUseCase : ICommandHandler<AddAwardCommand, Result<bool>>
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AddAwardUseCase(
            IMovieRepository movieRepository,
            IUnitOfWork unitOfWork)
        {
            _movieRepository = movieRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(AddAwardCommand command, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetByIdWithAwardAsync(command.Id);

            if (movie is null)
                return Result<bool>.AsFailure(Failure.NotFound("Filme", command.Id));

            AwardCategory? category;
            Institution? institution;

            try
            {
                category = AwardCategory.FromValue<AwardCategory>(command.CategoryId);
                institution = Institution.FromValue<Institution>(command.InstitutionId);
            }
            catch (InvalidOperationException ex)
            {
                return Result<bool>.AsFailure(
                    Failure.Validation($"Failed to resolve Smart Enum: {ex.Message}")
                );
            }
            if (category is null || institution is null)
                return Result<bool>.AsFailure(Failure.Validation(
                    "Failed to resolve one or more Smart Enum values from the provided IDs."));

            var awardResult = Award.Create(category, institution, command.Year);

            if (awardResult.IsFailure)
                return Result<bool>.AsFailure(awardResult.Failure!);

            var newMovieImage = awardResult.Success!;

            var domainResult = movie.AddAward(newMovieImage);

            if (domainResult.IsFailure)
                return Result<bool>.AsFailure(domainResult.Failure!);

            await _unitOfWork.Commit(cancellationToken);

            return Result<bool>.AsSuccess(true);
        }
    }
}
