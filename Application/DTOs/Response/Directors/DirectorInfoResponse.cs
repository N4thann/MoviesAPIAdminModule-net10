namespace Application.DTOs.Response.Directors
{
    public sealed record DirectorInfoResponse(
        Guid Id,
        string Name,
        DateTime BirthDate,
        string CountryName,
        string CountryCode,
        string? Biography,
        bool IsActive,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        int Age
        );
}
