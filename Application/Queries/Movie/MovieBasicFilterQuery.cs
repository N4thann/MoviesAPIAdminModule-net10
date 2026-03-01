using Application.Common;
using Application.DTOs.Response.Movies;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;
using Pandorax.PagedList;

namespace Application.Queries.Movie
{
    public record class MovieBasicFilterQuery(
    string? Title,
    string? OriginalTitle,
    string? CountryName,
    int? ReleaseYearBegin,
    int? ReleaseYearEnd,
    bool? Active,
    QueryStringParameters Parameters
    ) : IQuery<Result<IPagedList<MovieTableResponse>>>;
}
