using Application.Commands.Movie;
using Application.DTOs.Request.Movie;
using Application.DTOs.Response;
using Application.DTOs.Response.Movies;
using Application.Interfaces.Mediator;
using Application.Queries.Movie;
using Asp.Versioning;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MoviesAPIAdminModule.Filters;
using NSwag.Annotations;
using Pandorax.PagedList;

namespace MoviesAPIAdminModule.Controllers
{
    [ApiController]
    [EnableCors("AllowMyClient")] //Usar [DisableCors] para desativar em algum método específico
    [Route("api/v{version:apiVersion}/[controller]")]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [EnableRateLimiting("fixedwindow")]
    [OpenApiTag("Movies")]
    //[ApiVersion("1.0, Deprecared = true")] Para indicar que essa versão está depreciada e irá ser descontinuada no futuro
    //[ApiConventionType(typeof(DefaultApiConventions))] Caso não tivessemos retornos personalizados e fosse preciso um mais geral
    //[ApiExplorerSettings(IgnoreApi = true)] Caso eu quisesse ignora a documentação na interface do swagger dessa controller
    public class MovieController : BaseApiController
    {
        private readonly IMediator _mediator;

        public MovieController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// Cria um novo filme usando os dados fornecidos na requisição.
        /// </summary>
        /// <param name="request">Objeto contendo os detalhes do filme a ser criado. Não deve ser nulo.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> representando o resultado da operação. Retorna 201 Created com as informações básicas do filme em caso de sucesso; caso contrário, retorna 400/404/409 ou 500 conforme aplicável.</returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(MovieBasicInfoResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Obtém um filme pelo seu identificador (ID).
        /// </summary>
        /// <param name="id">O identificador único do filme.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> com os dados básicos do filme e status 200 OK em caso de sucesso; ou 400/404/500 em caso de erro.</returns>
        [HttpGet("GetMovieById/{id}")]
        [ProducesResponseType(typeof(MovieBasicInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetMovieById(Guid id, CancellationToken cancellationToken)
        {

            var command = new GetMovieByIdQuery(id);
            var result = await _mediator.Query<GetMovieByIdQuery, Result<MovieBasicInfoResponse>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var response = result.Success;

            return Ok(response);
        }

        /// <summary>
        /// Lista filmes aplicando filtros e paginação conforme os parâmetros fornecidos.
        /// </summary>
        /// <param name="request">Objeto de filtro com critérios como título, diretor, estúdio, país e faixa de ano de lançamento.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> com uma lista paginada de filmes filtrados e status 200 OK em caso de sucesso; ou 400/500 em caso de erro.</returns>
        [HttpGet("filtered")]
        [ProducesResponseType(typeof(IPagedList<MovieTableResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status500InternalServerError)]       
        public async Task<IActionResult> GetFilteredMovies([FromQuery] MovieBasicFilterRequest request, CancellationToken cancellationToken)
        {
            var query = new MovieBasicFilterQuery(
                request.Title,
                request.OriginalTitle,
                request.CountryName,
                request.ReleaseYearBegin,
                request.ReleaseYearEnd,
                request.Active,
                request
                );

            var result = await _mediator.Query<MovieBasicFilterQuery, Result<IPagedList<MovieTableResponse>>>(query, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            var pagedList = result.Success!;

            var response = new PagedResponse<MovieTableResponse>(
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
        /// Exclui um filme pelo seu identificador (ID).
        /// </summary>
        /// <param name="id">O identificador único do filme a ser excluído.</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> que retorna 204 NoContent em caso de exclusão bem-sucedida; ou 404/500 em caso de erro.</returns>
        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteMovie(Guid id, CancellationToken cancellationToken)
        {
            var command = new DeleteMovieCommand(id);

            var result =  await _mediator.Send<DeleteMovieCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }

        /// <summary>
        /// Adiciona múltiplos prêmios a um filme de uma só vez
        /// </summary>
        /// <param name="id">O identificador único do filme que receberá o prêmio.</param>
        /// <param name="request">Objeto contendo uma lista com os dados dos prêmios (categoria, instituição, ano).</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> que retorna 204 NoContent em caso de sucesso; ou 400/404/409/500 em caso de erro.</returns>
        [HttpPost("addAwardsToMovie/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddAwardsToMovie(Guid id, [FromBody] AddAwardsToMovieRequest request, CancellationToken cancellationToken)
        {
            var command = new AddAwardsToMovieCommand(id, request.Awards);

            var result = await _mediator.Send<AddAwardsToMovieCommand, Result<bool>>(command, cancellationToken);

            if (result.IsFailure)
                return HandleFailure(result.Failure!);

            return NoContent();
        }


        /// <summary>
        /// Envia uma imagem para o filme (Poster, Thumbnail ou Gallery).
        /// </summary>
        /// <param name="Id">O identificador único do filme que receberá a imagem.</param>
        /// <param name="request">Dados do arquivo de imagem e metadados (tipo de imagem, altText).</param>
        /// <param name="cancellationToken">Token que pode ser usado para cancelar a operação.</param>
        /// <returns>Um <see cref="IActionResult"/> que retorna 201 Created com o caminho/identificador da imagem em caso de sucesso; ou 400/404/409/500 em caso de erro.</returns>
        [HttpPost("uploadImage/{Id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(Failure), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

            if (response == null)
                return HandleFailure(Failure.Unknown);

            return Created(string.Empty, response);
        }
    }
}
