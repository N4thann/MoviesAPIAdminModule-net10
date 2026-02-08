using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Application.Queries.User;
using Domain.Identity;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Uses
{
    public class GetAllUsersQueryHandler : IQueryHandler<GetAllUsersQuery, Result<IEnumerable<UserSummaryResponse>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager) => _userManager = userManager;

        public async Task<Result<IEnumerable<UserSummaryResponse>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            // _userManager.Users retorna um IQueryable, o que é ótimo para performance.
            // O .Select garante que o Entity Framework gere uma consulta SQL
            // que busca APENAS as colunas que precisamos.
            var users = await _userManager.Users
                .Select(user => new UserSummaryResponse(
                    user.Id,
                    user.UserName!,
                    user.Email!,
                    user.PhoneNumber))
                .ToListAsync(cancellationToken);

            return Result<IEnumerable<UserSummaryResponse>>.AsSuccess(users);
        }
    }
}
