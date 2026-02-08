namespace Application.DTOs.Request.Studio
{
    public sealed record CreateStudioRequest(
        string Name,
        string CountryName,
        string CountryCode,
        DateTime FoundationDate,
        string? History = null
        );
}
