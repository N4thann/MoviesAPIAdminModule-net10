using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Application.Queries.Roles;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pandorax.PagedList;
using Pandorax.PagedList.EntityFrameworkCore;

namespace Application.UseCases.Roles
{
    public class GetAllRolesQueryHandler : IQueryHandler<GetAllRolesQuery, Result<IPagedList<RoleResponse>>>
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public GetAllRolesQueryHandler(RoleManager<IdentityRole> roleManager) => _roleManager = roleManager;

        public async Task<Result<IPagedList<RoleResponse>>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
        {
            var queryable = _roleManager.Roles.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.Name))
                queryable = queryable.Where(r => r.Name!.Contains(query.Name));

            var rolesPaged = await queryable
                .OrderBy(r => r.Name)
                .Select(r => new RoleResponse(r.Id, r.Name!))
                .ToPagedListAsync(query.Parameters.PageNumber, query.Parameters.PageSize, cancellationToken);

            return Result<IPagedList<RoleResponse>>.AsSuccess(rolesPaged);
        }
    }
}
