namespace Application.DTOs.Response.Studios
{
    public sealed record StudioTableResponse(
        Guid Id,
        string Name,
        string CountryName,
        DateTime FoundationDate,
        bool IsActive
    );
}
