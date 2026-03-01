
using Application.Commands.Movie;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Domain.SmartEnums;
using Domain.ValueObjects;

namespace Application.UseCases.Movies
{
    public class AddAwardUseCase : ICommandHandler<AddAwardsToMovieCommand, Result<bool>>
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

        public async Task<Result<bool>> Handle(AddAwardsToMovieCommand command, CancellationToken cancellationToken)
        {
            var movie = await _movieRepository.GetByIdWithAwardAsync(command.MovieId);

            if (movie is null)
                return Result<bool>.AsFailure(Failure.NotFound("Filme", command.MovieId));

            var awardsToCreate = new List<Award>();

            foreach (var item in command.Awards)
            {
                try
                {
                    var category = AwardCategory.FromValue<AwardCategory>(item.CategoryId);
                    var institution = Institution.FromValue<Institution>(item.InstitutionId);

                    var awardResult = Award.Create(category, institution, item.Year);
                    if (awardResult.IsFailure) return Result<bool>.AsFailure(awardResult.Failure!);

                    awardsToCreate.Add(awardResult.Success!);
                }
                catch (InvalidOperationException ex)
                {
                    return Result<bool>.AsFailure(Failure.Validation($"Erro ao processar prêmio: {ex.Message}"));
                }
            }

            var domainResult = movie.AddAwards(awardsToCreate);

            if (domainResult.IsFailure)
                return Result<bool>.AsFailure(domainResult.Failure!);

            await _unitOfWork.Commit(cancellationToken);
            return Result<bool>.AsSuccess(true);
        }
    }
}
