using Application.Commands.Studio;
using Application.DTOs.Request.Studio;
using Application.DTOs.Response;
using Application.DTOs.Response.Movies;
using Application.DTOs.Response.Studios;
using Application.Interfaces.Mediator;
using Application.Queries.Studio;
using Asp.Versioning;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesAPIAdminModule.Filters;
using Newtonsoft.Json;
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
    [OpenApiTag("Studio")]
    public class StudioController : BaseApiController
    {
        private readonly IMediator _mediator;

        public StudioController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Cria um novo estúdio usando os dados fornecidos na requisição.
        /// </summary>
        /// <param name="request">Objeto contendo os detalhes do estúdio a ser criado. Não deve ser nulo.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> representando o resultado da operação. Retorna 201 Created com as informações do estúdio em caso de sucesso; caso contrário, retorna 400 ou 500 conforme aplicável.</returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(StudioInfoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateStudio([FromBody] CreateStudioRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateStudioCommand(
                request.Name,
                request.CountryName,
                request.CountryCode,
                request.FoundationDate,
                request.History
                );

            var result = await _mediator.Send<CreateStudioCommand, Result<StudioInfoResponse>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success!;

            return CreatedAtAction(nameof(GetById),
                new {id = response.Id },
                response);
        }

        /// <summary>
        /// Obtém um estúdio pelo seu identificador (ID).
        /// </summary>
        /// <param name="id">O identificador único do estúdio.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> com os dados do estúdio e status 200 OK em caso de sucesso; ou 404/500 em caso de erro.</returns>
        [HttpGet("getById/{id}")]
        [ProducesResponseType(typeof(StudioInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) 
        {
            var query = new GetStudioByIdQuery(id);
            var result = await _mediator.Query<GetStudioByIdQuery, Result<StudioInfoResponse>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success!;

            return Ok(response);
        }

        /// <summary>
        /// Lista estúdios aplicando filtros e paginação conforme os parâmetros fornecidos.
        /// </summary>
        /// <param name="request">Objeto de filtro com critérios como nome, país, ano de fundação e indicador de ativo.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> com uma lista paginada de estúdios filtrados e status 200 OK em caso de sucesso; ou 400/500 em caso de erro.</returns>
        [HttpGet("filtered")]
        [ProducesResponseType(typeof(IPagedList<StudioTableResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFilteredStudios([FromQuery] StudioFilterRequest request, CancellationToken cancellationToken)
        {
            var query = new StudioFilterQuery(
                request.Name,
                request.CountryName,
                request.FoundationYearBegin,
                request.FoundationYearEnd,
                request.Active,
                request
                );

            var result = await _mediator.Query<StudioFilterQuery, Result<IPagedList<StudioTableResponse>>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var pagedList = result.Success!;

            var response = new PagedResponse<StudioTableResponse>(
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

        //[HttpPatch("{id}")]
        //[ProducesResponseType(typeof(StudioInfoResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[OpenApiOperation("Atualiza parcialmente um estúdio com o JsonPatchDocument")]
        //[OpenApiTag("Roles Commands")]
        //public async Task<IActionResult> UpdatePatchStudio(
        //    Guid id,
        //    [FromBody] JsonPatchDocument<Studio> patchDoc,
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

        //    var command = new PatchStudioCommand(id, patchDoc);
        //    var response = await _mediator.Send<PatchStudioCommand, StudioInfoResponse>(command, cancellationToken);

        //    return Ok(response);
        //}

        /// <summary>
        /// Exclui um estúdio pelo seu identificador (ID).
        /// </summary>
        /// <param name="id">O identificador único do estúdio a ser excluído.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> que retorna 204 NoContent em caso de exclusão bem-sucedida; ou 404/500 em caso de erro.</returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteStudio(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteStudioCommand(id);

            var result = await _mediator.Send<DeleteStudioCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }
    }
}
