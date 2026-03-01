using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;
using Pandorax.PagedList;

namespace Application.Queries.Roles
{
    public record GetAllRolesQuery(
        string? Name,
        QueryStringParameters Parameters
    ) : IQuery<Result<IPagedList<RoleResponse>>>;
}
