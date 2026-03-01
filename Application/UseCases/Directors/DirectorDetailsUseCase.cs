using Application.DTOs.Response.Directors;
using Application.Interfaces.Mediator;
using Application.Queries.Director;
using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Mapster;

namespace Application.UseCases.Directors
{
    public class DirectorDetailsUseCase : IQueryHandler<DetailsDirectorQuery, Result<DirectorDetailsResponse>>
    {
        private readonly IRepository<Director> _directorRepository;
        private readonly IMovieRepository _movieRepository;

        public DirectorDetailsUseCase(
            IRepository<Director> directorRepository,
            IMovieRepository movieRepository)
        {
            _directorRepository = directorRepository;
            _movieRepository = movieRepository;
        }

        public async Task<Result<DirectorDetailsResponse>> Handle(DetailsDirectorQuery query, CancellationToken cancellationToken)
        {
            var director = await _directorRepository.GetByIdAsync(query.Id);

            if (director == null)
                return Result<DirectorDetailsResponse>.AsFailure(Failure.NotFound("Director", query.Id));

            var movies = await _movieRepository.GetMoviesByDirectorIdAsync(query.Id);

            var response = director.Adapt<DirectorDetailsResponse>();

            var movieSummaries = movies.Select(m => new MovieSummaryResponse(
                m.Id,
                m.Name, 
                m.Genre.Name,
                m.ReleaseYear,
                m.Awards.Select(a => a.Adapt<AwardResponse>()).ToList()
            )).ToList();

            var finalResponse = response with { Movies = movieSummaries };

            return Result<DirectorDetailsResponse>.AsSuccess(finalResponse);
        }

    }
}
