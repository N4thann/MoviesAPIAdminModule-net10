using Application.DTOs.Response.Movies;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;

namespace Application.Queries.Movie
{
    public record class GetMovieByIdQuery(
        Guid Id
        ) : IQuery<Result<MovieBasicInfoResponse>>;
}
