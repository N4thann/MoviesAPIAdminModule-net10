using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Application.Queries.User;
using Domain.Identity;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pandorax.PagedList;
using Pandorax.PagedList.EntityFrameworkCore;

namespace Application.UseCases.Uses
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Result<IPagedList<UserSummaryResponse>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

        public async Task<Result<IPagedList<UserSummaryResponse>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            var queryable = _userManager.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query.UserName))
                queryable = queryable.Where(u => u.UserName!.Contains(query.UserName));

            if (!string.IsNullOrWhiteSpace(query.Email))
                queryable = queryable.Where(u => u.Email!.Contains(query.Email));

            var usersPaged = await queryable
            .OrderBy(u => u.UserName)
            .Select(user => new UserSummaryResponse(
                user.Id,
                user.UserName!,
                user.Email!,
                user.PhoneNumber))
            .ToPagedListAsync(query.Parameters.PageNumber, query.Parameters.PageSize, cancellationToken);

            return Result<IPagedList<UserSummaryResponse>>.AsSuccess(usersPaged);
        }
    }
}
