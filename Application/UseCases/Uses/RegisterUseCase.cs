using Application.Commands.User;
using Application.Interfaces.Mediator;
using Domain.Identity;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Uses
{
    public class RegisterUseCase : ICommandHandler<RegisterCommand, Result<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; //Caso tenha alguma implementação futura

        public RegisterUseCase(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<Result<bool>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            var userByUserName = await _userManager.FindByNameAsync(command.UserName);

            if (userByUserName != null)
                return Result<bool>.AsFailure(Failure.Conflict($"Username '{command.UserName}' is already taken."));

            var userByEmail = await _userManager.FindByEmailAsync(command.Email);

            if (userByEmail != null)
                return Result<bool>.AsFailure(Failure.Conflict($"Email '{command.Email}' is already in use."));

            ApplicationUser user = new()
            {
                Email = command.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = command.UserName,
                PhoneNumber = command.PhoneNumber
            };

            var identityResult = await _userManager.CreateAsync(user, command.Password);

            if (!identityResult.Succeeded)
            {
                var errors = string.Join("\n", identityResult.Errors.Select(e => e.Description));
                return Result<bool>.AsFailure(Failure.Validation(errors));
            }

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Admin");

            if (!roleResult.Succeeded)
            {
                var errors = string.Join("\n", roleResult.Errors.Select(e => e.Description));
                return Result<bool>.AsFailure(Failure.Infrastructure($"User created, but failed to assign Admin role: {errors}"));
            }

            return Result<bool>.AsSuccess(true);
        }
    }
}
