using Application.Commands.Studio;
using Application.DTOs.Request.Studio;
using Application.DTOs.Response;
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
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    public class StudioController : BaseApiController
    {
        private readonly IMediator _mediator;

        public StudioController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        [ProducesResponseType(typeof(StudioInfoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Cria um novo estúdio")]
        [OpenApiTag("Studio")]
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

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(StudioInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Obtém um estúdio por ID")]
        [OpenApiTag("Studio")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) 
        {
            var query = new GetStudioByIdQuery(id);
            var result = await _mediator.Query<GetStudioByIdQuery, Result<StudioInfoResponse>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success!;

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<StudioInfoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Lista todos os estúdios")]
        [OpenApiTag("Studio")]
        public async Task<IActionResult> GetAllPagination([FromQuery] StudioParametersRequest parameters, CancellationToken cancellationToken)
        {
            var query = new ListStudiosQuery(parameters);
            var result = await _mediator.Query<ListStudiosQuery, Result<IPagedList<StudioInfoResponse>>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success!;

            var metadata = new
            {
                response.Count,
                response.PageSize,
                response.PageIndex,
                response.TotalPageCount,
                response.TotalItemCount,
                response.HasNextPage,
                response.HasPreviousPage
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

            return Ok(response);
        }

        [HttpGet("filtered")]
        [ProducesResponseType(typeof(IPagedList<StudioInfoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Lista estúdios com filtros e paginação")]
        [OpenApiTag("Studio")]
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

            var result = await _mediator.Query<StudioFilterQuery, Result<IPagedList<StudioInfoResponse>>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success!;

            var metadata = new
            {
                response.Count,
                response.PageSize,
                response.PageIndex,
                response.TotalPageCount,
                response.TotalItemCount,
                response.HasNextPage,
                response.HasPreviousPage
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));

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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Exclui um estúdio por ID")]
        [OpenApiTag("Studio")]
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
