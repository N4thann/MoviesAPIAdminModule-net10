namespace Application.DTOs.Response.Studios
{
    public sealed record StudioInfoResponse(
        Guid Id,
        string Name,
        string CountryName,
        string CountryCode,
        DateTime FoundationDate,
        string? History,
        bool IsActive,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        int YearsInOperation
        );
}
