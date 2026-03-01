using Application.DTOs.Response.Movies;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Commands.Movie
{
    public record PatchMovieCommand(
        Guid Id,
        JsonPatchDocument<Domain.Entities.Movie> PatchDocument
        ) : ICommand<Result<MovieBasicInfoResponse>>;
}
