using Application.Commands.Movie;
using Application.DTOs.Mappings;
using Application.DTOs.Response.Directors;
using Application.DTOs.Response.Movies;
using Application.Interfaces.Mediator;
using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Domain.ValueObjects;
using Mapster;

namespace Application.UseCases.Movies
{
    public class CreateMovieUseCase : ICommandHandler<CreateMovieCommand, Result<MovieBasicInfoResponse>>
    {
        private readonly IRepository<Movie> _repositoryMovie;
        private readonly IRepository<Studio> _repositoryStudio;
        private readonly IRepository<Director> _repositoryDirector;
        private readonly IUnitOfWork _unitOfWork;

        public CreateMovieUseCase(IRepository<Movie> repositoryMovie,
            IRepository<Studio> repositoryStudio,
            IRepository<Director> repositoryDirector,
            IUnitOfWork unitOfWork)
        {
            _repositoryMovie = repositoryMovie;
            _repositoryStudio = repositoryStudio;
            _repositoryDirector = repositoryDirector;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<MovieBasicInfoResponse>> Handle(CreateMovieCommand command, CancellationToken cancellationToken)
        {
            var countryResult = Country.Create(command.CountryName, command.CountryCode);
            var boxOfficeResult = Money.Create(command.BoxOfficeAmount, command.BoxOfficeCurrency);
            var budgetResult = Money.Create(command.BudgetAmount, command.BudgetCurrency);
            var genreResult = Genre.Create(command.GenreName, command.GenreDescription);
            var durationResult = Duration.Create(command.DurationMinutes);

            var voValidationResult = countryResult.Combine(boxOfficeResult, budgetResult, genreResult, durationResult);

            if (voValidationResult.IsFailure)
                return Result<MovieBasicInfoResponse>.AsFailure(voValidationResult.Failure!);

            var studio = await _repositoryStudio.GetByIdAsync(command.StudioId);
            var director = await _repositoryDirector.GetByIdAsync(command.DirectorId);

            if (studio == null)
                return Result<MovieBasicInfoResponse>.AsFailure(Failure.NotFound("Studio", command.StudioId));

            if (director == null)
                return Result<MovieBasicInfoResponse>.AsFailure(Failure.NotFound("Director", command.DirectorId));


            var movieCreateResult = Movie.Create(
                command.Title,
                command.OriginalTitle,
                command.Synopsis,
                command.ReleaseYear,
                durationResult.Success!, 
                countryResult.Success!,
                studio,
                director,
                genreResult.Success!,
                boxOfficeResult.Success,
                budgetResult.Success
            );

            if (movieCreateResult.IsFailure)
                return Result<MovieBasicInfoResponse>.AsFailure(movieCreateResult.Failure!);

            var movie = movieCreateResult.Success!;

            _repositoryMovie.Add(movie);
            await _unitOfWork.Commit(cancellationToken);

            var response = movie.Adapt<MovieBasicInfoResponse>();

            return Result<MovieBasicInfoResponse>.AsSuccess(response);
        }
    }
}
