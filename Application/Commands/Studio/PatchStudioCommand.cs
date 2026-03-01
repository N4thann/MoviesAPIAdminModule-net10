using Application.DTOs.Response.Studios;
using Application.Interfaces.Mediator;
using Microsoft.AspNetCore.JsonPatch;

namespace Application.Commands.Studio
{
    public record PatchStudioCommand(
        Guid Id,
        JsonPatchDocument<Domain.Entities.Studio> PatchDocument //Estava dando conflito com nome da Pasta, então tive que mapear colocando o namespace
    ) : ICommand<StudioInfoResponse>;
}
