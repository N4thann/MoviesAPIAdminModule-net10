using Application.Commands.User;
using Application.DTOs.Authentication;
using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Application.Queries.User;
using Asp.Versioning;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesAPIAdminModule.Filters;
using NSwag.Annotations;

namespace MoviesAPIAdminModule.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/users")]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [OpenApiTag("Users")]
    public class UsersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(IEnumerable<UserSummaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)] // Se o usuário não for Admin
        [OpenApiOperation("(Admin) Lista todos os usuários registrados no sistema.")]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var query = new GetAllUsersQuery();
            var result = await _mediator.Query<GetAllUsersQuery, Result<IEnumerable<UserSummaryResponse>>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return Ok(result.Success);
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Cria uma nova conta de usuário no sistema")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(request.UserName!, request.Email!, request.Password!, request.PhoneNumber);

            var result = await _mediator.Send<RegisterCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }

        [HttpPost("{username}/Revoke")]
        [Authorize(Policy = "ExclusivePolicyOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("(Admin) Invalida a sessão de um usuário, forçando um novo login")]
        public async Task<IActionResult> Revoke(string username, CancellationToken cancellationToken)
        {
            var command = new RevokeByUsernameCommand(username);

            var result = await _mediator.Send<RevokeByUsernameCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }
    }
}
