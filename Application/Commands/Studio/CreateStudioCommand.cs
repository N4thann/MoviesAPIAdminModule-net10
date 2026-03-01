using Application.DTOs.Response.Studios;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;

namespace Application.Commands.Studio
{
    public record class CreateStudioCommand(
        string Name,
        string CountryName,
        string CountryCode,
        DateTime FoundationDate,
        string? History = null
        ) : ICommand<Result<StudioInfoResponse>>;
}
