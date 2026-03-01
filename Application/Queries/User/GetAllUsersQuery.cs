using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;
using Pandorax.PagedList;

namespace Application.Queries.User
{
    public record GetAllUsersQuery(
        string? UserName,
        string? Email,
        QueryStringParameters Parameters
    ) : IQuery<Result<IPagedList<UserSummaryResponse>>>;
}
