using Application.Commands.Role;
using Application.DTOs.Authentication;
using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Application.Queries.Roles;
using Asp.Versioning;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MoviesAPIAdminModule.Filters;
using NSwag.Annotations;

namespace MoviesAPIAdminModule.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/roles")]
    [EnableCors("AllowMyClient")]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [OpenApiTag("Roles")]
    public class RolesController : BaseApiController
    {
        private readonly IMediator _mediator;
        public RolesController(IMediator mediator) => _mediator = mediator;

        [HttpPost("create")]
        [Authorize(Policy = "ExclusivePolicyOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateRoleCommand(request.RoleName);
            var result = await _mediator.Send<CreateRoleCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }

        [HttpPost("add-user-to-role")]
        [Authorize(Policy = "SuperAdminOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddUserToRole([FromBody] AddUserToRoleRequest request, CancellationToken cancellationToken)
        {
            var command = new AddUserToRoleCommand(request.Email, request.RoleName);
            var result = await _mediator.Send<AddUserToRoleCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(IEnumerable<RoleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
        {
            var query = new GetAllRolesQuery();
            var result = await _mediator.Query<GetAllRolesQuery, Result<IEnumerable<RoleResponse>>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return Ok(result.Success);
        }
    }
}
