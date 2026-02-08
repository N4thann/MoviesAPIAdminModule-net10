using Domain.Entities;

namespace Application.DTOs.Request.Director
{
    public sealed record CreateDirectorRequest(
        string Name,
        DateTime BirthDate,
        string CountryName,
        string CountryCode, 
        string? Biography = null,
        Gender Gender = Gender.NotSpecified
        );
}
