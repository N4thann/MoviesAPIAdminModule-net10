using Application.Commands.User;
using Application.DTOs.Authentication;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Application.Queries.User;
using Asp.Versioning;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesAPIAdminModule.Filters;
using NSwag.Annotations;
using Pandorax.PagedList;

namespace MoviesAPIAdminModule.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [OpenApiTag("Users")]
    public class UsersController : BaseApiController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Retorna uma lista paginada de usuários aplicando filtros opcionais de nome de usuário e email.
        /// Requer autorização com a política "AdminOnly".
        /// </summary>
        /// <param name="request">Objeto contendo filtros (UserName, Email) e parâmetros de paginação.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>
        /// Retorna 200 (OK) com um <see cref="PagedResponse{UserSummaryResponse}"/> em caso de sucesso;
        /// 401 (Unauthorized) se o usuário não estiver autenticado;
        /// 403 (Forbidden) se o usuário não tiver a permissão necessária.
        /// Em caso de falha operacional, retorna a Failure apropriada conforme o pipeline de erros.
        /// </returns>
        [HttpGet("filtered")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(PagedResponse<UserSummaryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll([FromQuery] UserFilterRequest request, CancellationToken cancellationToken)
        {
            var query = new GetAllUsersQuery(request.UserName, request.Email, request);

            var result = await _mediator.Query<GetAllUsersQuery, Result<IPagedList<UserSummaryResponse>>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var pagedList = result.Success!;

            var response = new PagedResponse<UserSummaryResponse>(
                Items: pagedList,
                CurrentPage: pagedList.PageIndex,
                PageSize: pagedList.PageSize,
                TotalCount: pagedList.TotalItemCount,
                TotalPages: pagedList.TotalPageCount,
                HasNext: pagedList.HasNextPage,
                HasPrevious: pagedList.HasPreviousPage
            );

            return Ok(response);
        }

        /// <summary>
        /// Cria uma nova conta de usuário no sistema com as credenciais fornecidas.
        /// </summary>
        /// <param name="request">Objeto contendo os dados de registro (username, email, senha, telefone).</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> que retorna 204 NoContent em caso de sucesso; ou 400/409/500 em caso de falha.</returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
        {
            var command = new RegisterCommand(request.UserName!, request.Email!, request.Password!, request.PhoneNumber);

            var result = await _mediator.Send<RegisterCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }

        /// <summary>
        /// Invalida a sessão de um usuário, forçando-o a realizar um novo login (requer permissão específica).
        /// </summary>
        /// <param name="username">Nome de usuário cuja sessão será invalidada.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> que retorna 204 NoContent em caso de sucesso; ou 401/500 em caso de erro.</returns>
        [HttpPost("revoke/{username}")]
        [Authorize(Policy = "ExclusivePolicyOnly")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]
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