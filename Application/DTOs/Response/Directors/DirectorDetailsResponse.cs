namespace Application.DTOs.Response.Directors
{
    public sealed record DirectorDetailsResponse(
        Guid Id,
        string Name,
        string? Biography,
        int Age,
        string CountryName,
        List<MovieSummaryResponse> Movies
    );

    public sealed record MovieSummaryResponse(
        Guid Id,
        string Title,
        string GenreName,
        int ReleaseYear,
        List<AwardResponse> Awards
    );

    public sealed record AwardResponse(
        string Category,
        string Institution,
        int Year
    );
}
