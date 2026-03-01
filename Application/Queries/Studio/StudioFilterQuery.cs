using Application.Common;
using Application.DTOs.Response.Studios;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;
using Pandorax.PagedList;

namespace Application.Queries.Studio
{
    public record class StudioFilterQuery(
        string? Name,
        string? CountryName,
        int? FoundationYearBegin,
        int? FoundationYearEnd,
        bool? Active,
        QueryStringParameters Parameters
    ) : IQuery<Result<IPagedList<StudioTableResponse>>>;
}
