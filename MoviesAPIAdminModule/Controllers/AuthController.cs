using Application.Commands.Authentication;
using Application.DTOs.Authentication;
using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Asp.Versioning;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MoviesAPIAdminModule.Filters;
using NSwag.Annotations;

namespace MoviesAPIAdminModule.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [EnableCors("AllowMyClient")]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [OpenApiTag("Authentication")]
    public class AuthController : BaseApiController
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpPost("login")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Autentica um usuário e retorna os tokens de acesso e atualização")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
        {
            var command = new LoginCommand(request.UserName!, request.Password!);

            var result = await _mediator.Send<LoginCommand, Result<TokenResponse>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return Ok(result.Success);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Emite um novo token de acesso utilizando um token de atualização válido")]
        public async Task<IActionResult> RefreshToken(TokenRequest request, CancellationToken cancellationToken)
        {
            var command = new RefreshTokenCommand(request.AccessToken!, request.RefreshToken!);

            var result = await _mediator.Send<RefreshTokenCommand, Result<TokenResponse>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return Ok(result.Success);
        }                
    }
}
