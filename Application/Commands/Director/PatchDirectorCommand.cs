using Application.DTOs.Response.Directors;
using Application.Interfaces.Mediator;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Commands.Director
{
    public record class PatchDirectorCommand(
        Guid Id,
        JsonPatchDocument<Domain.Entities.Director> PatchDocument
        ) : ICommand<DirectorTableResponse>;
}
