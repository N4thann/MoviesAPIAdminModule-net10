using Application.DTOs.Response.Movies;
using Application.Interfaces.Mediator;
using Application.Queries.Movie;
using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Mapster;

namespace Application.UseCases.Movies
{
    public class GetMovieByIdUseCase : IQueryHandler<GetMovieByIdQuery, Result<MovieBasicInfoResponse>>
    {
        private readonly IRepository<Movie> _repository;

        public GetMovieByIdUseCase(IRepository<Movie> repository) => _repository = repository;

        public async Task<Result<MovieBasicInfoResponse>> Handle(GetMovieByIdQuery query, CancellationToken cancellationToken)
        {
            var movie = await _repository.GetByIdAsync(query.Id);

            if (movie == null)
                return Result<MovieBasicInfoResponse>.AsFailure(Failure.NotFound("Movie", query.Id));

            var response = movie.Adapt<MovieBasicInfoResponse>();

            return Result<MovieBasicInfoResponse>.AsSuccess(response);
        }
    }
}
