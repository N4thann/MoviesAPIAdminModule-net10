using Application.Commands.Director;
using Application.DTOs.Request.Director;
using Application.DTOs.Response;
using Application.DTOs.Response.Directors;
using Application.Interfaces.Mediator;
using Application.Queries.Director;
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
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [OpenApiTag("Director")]
    public class DirectorController : BaseApiController
    {
        private readonly IMediator _mediator;
        public DirectorController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Cria um novo diretor usando os dados fornecidos na requisição.
        /// </summary>
        /// <param name="request">Objeto contendo os detalhes do diretor a ser criado. Não deve ser nulo.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> representando o resultado da operação. Retorna 201 Created com as informações do diretor em caso de sucesso; caso contrário, retorna 400 Bad Request ou 500 Internal Server Error.</returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(DirectorInfoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDirector([FromBody] CreateDirectorRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateDirectorCommand(
                request.Name,
                request.BirthDate,
                request.CountryName,
                request.CountryCode,
                request.Biography,
                request.Gender
            );

            var result = await _mediator.Send<CreateDirectorCommand, Result<DirectorInfoResponse>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success!;

            return CreatedAtAction(nameof(GetById),
                new { id = response.Id },
                response);
        }

        //[HttpPatch("{id}")]
        //[ProducesResponseType(typeof(DirectorInfoResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[OpenApiTag("Director Commands")]
        //public async Task<IActionResult> UpdatePatchDirector(
        //    Guid id,
        //    [FromBody] JsonPatchDocument<Director> patchDoc,
        //    CancellationToken cancellationToken)
        //{
        //    if (patchDoc == null)
        //    {
        //        return BadRequest("Patch document cannot be null.");
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    var command = new PatchDirectorCommand(id, patchDoc);
        //    var response = await _mediator.Send<PatchDirectorCommand, DirectorInfoResponse>(command, cancellationToken);

        //    return Ok(response);
        //}

        /// <summary>
        /// Obtém um diretor pelo seu identificador (ID).
        /// </summary>
        /// <param name="id">O identificador único do diretor.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> com os dados do diretor e status 200 OK em caso de sucesso; ou 404/500 em caso de erro.</returns>
        [HttpGet("{id}")]       
        [ProducesResponseType(typeof(DirectorInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var query = new GetDirectorByIdQuery(id);

            var result = await _mediator.Query<GetDirectorByIdQuery, Result<DirectorInfoResponse>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success!;

            return Ok(response);
        }

        /// <summary>
        /// Obtém os detalhes completos de um diretor, incluindo sua biografia e a lista de filmes dirigidos com seus respectivos prêmios.
        /// </summary>
        /// <param name="id">O identificador único do diretor.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação.</param>
        /// <returns>Retorna um <see cref="DirectorDetailsResponse"/> em caso de sucesso, ou falha se o diretor não for encontrado.</returns>
        [HttpGet("{id}/details")]
        [ProducesResponseType(typeof(DirectorDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDetails(Guid id, CancellationToken cancellationToken)
        {
            var query = new DetailsDirectorQuery(id);
            var result = await _mediator.Query<DetailsDirectorQuery, Result<DirectorDetailsResponse>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return Ok(result.Success);
        }

        /// <summary>
        /// Lista diretores aplicando filtros e paginação conforme os parâmetros fornecidos.
        /// </summary>
        /// <param name="request">Objeto de filtro contendo critérios como nome, país, faixa etária e indicador de ativo.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> com uma lista paginada de diretores filtrados e status 200 OK em caso de sucesso; ou 400/500 em caso de erro.</returns>
        [HttpGet("filtered")]
        [ProducesResponseType(typeof(IPagedList<DirectorTableResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]       
        public async Task<IActionResult> GetFilteredDirectors([FromQuery] DirectorFilterRequest request, CancellationToken cancellationToken)
        {
            var query = new DirectorFilterQuery(
                request.Name,
                request.CountryName,
                request.AgeBegin,
                request.AgeEnd,
                request.Active,
                request
                );

            var result = await _mediator.Query<DirectorFilterQuery, Result<IPagedList<DirectorTableResponse>>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var pagedList = result.Success!;

            var response = new PagedResponse<DirectorTableResponse>(
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
        /// Exclui um diretor pelo seu identificador (ID).
        /// </summary>
        /// <param name="id">O identificador único do diretor a ser excluído.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> que retorna 204 NoContent em caso de exclusão bem-sucedida; ou 404/500 em caso de erro.</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteDirector(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteDirectorCommand(id);

            var result = await _mediator.Send<DeleteDirectorCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }
    }
}
