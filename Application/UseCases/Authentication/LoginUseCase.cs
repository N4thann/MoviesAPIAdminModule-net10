using Application.Commands.Authentication;
using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Mediator;
using Domain.Enums;
using Domain.Identity;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Application.UseCases.Authentication
{
    public class LoginUseCase : ICommandHandler<LoginCommand, Result<TokenResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly JwtOptions _jwtOptions;

        private readonly List<string> _adminRoles = new() { "Admin", "SuperAdmin" };

        public LoginUseCase(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<Result<TokenResponse>> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByNameAsync(command.UserName);

            if (user is null || !await _userManager.CheckPasswordAsync(user, command.Password))
                return Result<TokenResponse>.AsFailure(Failure.InvalidCredentials);

            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles == null || !userRoles.Any(role => _adminRoles.Contains(role)))
            {
                return Result<TokenResponse>.AsFailure(
                    new Failure(FailureType.Forbidden, "User does not have the required role to access this module.")
                );
            }

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("id", user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAccessToken(authClaims, _jwtOptions);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenValidityInMinutes);
            user.RefreshToken = refreshToken;

            var updateResult = await _userManager.UpdateAsync(user);

            if(!updateResult.Succeeded)
            {
               var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
               return Result<TokenResponse>.AsFailure(Failure.Infrastructure($"Failed to save refresh token: {errors}"));
            }

            var response = new TokenResponse(
                new JwtSecurityTokenHandler().WriteToken(token),
                refreshToken,
                token.ValidTo
            );

            return Result<TokenResponse>.AsSuccess(response);
        }
    }
}
