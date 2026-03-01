using Application.Commands.Authentication;
using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Mediator;
using Domain.Identity;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

namespace Application.UseCases.Authentication
{
    public class RefreshTokenUseCase : ICommandHandler<RefreshTokenCommand, Result<TokenResponse>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly JwtOptions _jwtOptions;

        public RefreshTokenUseCase(
            UserManager<ApplicationUser> userManager,
            ITokenService tokenService,
            IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(command.AccessToken, _jwtOptions);

            if (principal?.Identity?.Name is null)
                return Result<TokenResponse>.AsFailure(Failure.Unauthorized("Invalid access token."));

            string userName = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName!);

            if (user is null || user.RefreshToken != command.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Result<TokenResponse>.AsFailure(Failure.Unauthorized("Invalid refresh token or session."));

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _jwtOptions);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(_jwtOptions.RefreshTokenValidityInMinutes);

            await _userManager.UpdateAsync(user);

            var response = new TokenResponse(
                new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                newRefreshToken,
                newAccessToken.ValidTo
            );

            return Result<TokenResponse>.AsSuccess(response);
        }
    }
}
