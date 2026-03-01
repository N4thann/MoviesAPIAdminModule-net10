using Application.DTOs.Response.Directors;
using Application.Interfaces.Mediator;
using Domain.Entities;
using Domain.SeedWork.Core;

namespace Application.Commands.Director
{
    public record CreateDirectorCommand(
        string Name,
        DateTime BirthDate,
        string CountryName,
        string CountryCode,
        string? Biography = null,
        Gender Gender = Gender.NotSpecified
    ) : ICommand<Result<DirectorInfoResponse>>;
}
