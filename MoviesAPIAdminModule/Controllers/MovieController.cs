using Application.Commands.Movie;
using Application.DTOs.Request.Movie;
using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Application.Queries.Movie;
using Asp.Versioning;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MoviesAPIAdminModule.Filters;
using Newtonsoft.Json;
using NSwag.Annotations;
using Pandorax.PagedList;

namespace MoviesAPIAdminModule.Controllers
{
    [ApiController]
    [EnableCors("PoliticaCORS1")] //Usar [DisableCors] para desativar em algum método específico
    [Route("api/v{version:apiVersion}/[controller]/[action]")]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [EnableRateLimiting("fixedwindow")]
    //[ApiVersion("1.0, Deprecared = true")] Para indicar que essa versão está depreciada e irá ser descontinuada no futuro
    //[ApiConventionType(typeof(DefaultApiConventions))] Caso não tivessemos retornos personalizados e fosse preciso um mais geral
    //[ApiExplorerSettings(IgnoreApi = true)] Caso eu quisesse ignora a documentação na interface do swagger dessa controller
    public class MovieController : BaseApiController
    {
        private readonly IMediator _mediator;

        public MovieController(IMediator mediator) => _mediator = mediator;

        //[MapToApiVersion(2)] Utilizando para mapear o método action informando que apenas na versão 2 poderá ser acessado esse action
        [HttpPost]
        [ProducesResponseType(typeof(MovieBasicInfoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Cria um novo filme")]
        [OpenApiTag("Movies")]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateMovieCommand(
                request.Title,
                request.OriginalTitle,
                request.Synopsis,
                request.ReleaseYear,
                request.DurationMinutes,
                request.CountryName,
                request.CountryCode,
                request.GenreName,
                request.GenreDescription,
                request.BoxOfficeAmount,
                request.BoxOfficeCurrency,
                request.BudgetAmount,
                request.BudgetCurrency,
                request.DirectorId,
                request.StudioId
                );

            var result = await _mediator.Send<CreateMovieCommand, Result<MovieBasicInfoResponse>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success;

            return CreatedAtAction(nameof(GetMovieById),
                new { id = response!.Id },
                response);
        }

        //[HttpPatch("{id}")]
        //[ProducesResponseType(typeof(MovieBasicInfoResponse), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status406NotAcceptable)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //[OpenApiOperation("Atualiza parcialmente um filme com o JsonPatchDocument")]
        //[OpenApiTag("Movies Commands")]
        //public async Task<IActionResult> UpdatePatchMovie(Guid id, [FromBody] JsonPatchDocument<Movie> patchDoc, CancellationToken cancellationToken)
        //{
        //    if (patchDoc == null)
        //    {
        //        return BadRequest("Patch document cannot be null.");
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    var command = new PatchMovieCommand(id, patchDoc);
        //    var response = await _mediator.Send<PatchMovieCommand, MovieBasicInfoResponse>(command, cancellationToken);
        //    return Ok(response);
        //}

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MovieBasicInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Obtém um filme por ID")]
        [OpenApiTag("Movies")]
        public async Task<IActionResult> GetMovieById(Guid id, CancellationToken cancellationToken)
        {

            var command = new GetMovieByIdQuery(id);
            var result = await _mediator.Query<GetMovieByIdQuery, Result<MovieBasicInfoResponse>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success;

            return Ok(response);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IPagedList<MovieBasicInfoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Lista todos os filmes")]
        [OpenApiTag("Movies")]
        public async Task<IActionResult> GetAllPagination([FromQuery] MovieParametersRequest parameters, CancellationToken cancellationToken)
        {
            var query = new ListMoviesQuery(parameters);
            var result = await _mediator.Query<ListMoviesQuery, Result<IPagedList<MovieBasicInfoResponse>>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success;

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
        [ProducesResponseType(typeof(IPagedList<MovieBasicInfoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Lista filmes com filtros e paginação")]
        [OpenApiTag("Movies")]
        public async Task<IActionResult> GetFilteredMovies([FromQuery] MovieBasicFilterRequest request, CancellationToken cancellationToken)
        {
            var query = new MovieBasicFilterQuery(
                request.Title,
                request.OriginalTitle,
                request.CountryName,
                request.ReleaseYearBegin,
                request.ReleaseYearEnd,
                request.DirectorName,
                request.StudioName,
                request.GenreName,
                request
                );

            var result = await _mediator.Query<MovieBasicFilterQuery, Result<IPagedList<MovieBasicInfoResponse>>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success;

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

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Exclui um filme por ID")]
        [OpenApiTag("Movies")]
        public async Task<IActionResult> DeleteMovie(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteMovieCommand(id);

            var result =  await _mediator.Send<DeleteMovieCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }

        [HttpPost("{Id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Adiciona um prêmio ao filme")]
        [OpenApiTag("Movies")]
        public async Task<IActionResult> AddAwardToMovie(Guid Id, [FromBody] AwardRequest request, CancellationToken cancellationToken)
        {
            var command = new AddAwardCommand(
                Id,
                request.CategoryId,
                request.InstitutionId,
                request.Year
                );

            var result = await _mediator.Send<AddAwardCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }


        [HttpPost("{Id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [OpenApiOperation("Adiciona uma imagem que pode ser do tipo Poster, Thumbnail ou Gallery ao filme")]
        [OpenApiTag("Movies")]
        public async Task<IActionResult> UploadImage(Guid Id, [FromForm] UploadImageRequest request, CancellationToken cancellationToken)
        {
            if (request.ImageFile == null || request.ImageFile.Length == 0)
            {
                return BadRequest(Failure.Validation("Nenhum arquivo de imagem foi enviado."));
            }

            var command = new AddMovieImageCommand(
                Id,
                request.ImageFile.OpenReadStream(),
                request.ImageFile.FileName,
                request.ImageFile.ContentType,
                request.ImageType,
                request.AltText
            );

            var result = await _mediator.Send<AddMovieImageCommand, Result<string>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success;

            return Ok(response);
        }
    }
}
