namespace Application.DTOs.Response.Directors
{
    public sealed record DirectorTableResponse(
        Guid Id,
        string Name,
        string CountryName,
        bool IsActive,
        int Age
        );
}
