namespace Application.DTOs.Response.Movies
{
    public sealed record MovieBasicInfoResponse(
        Guid Id,
        string Title,
        string OriginalTitle,
        string Synopsis,
        int ReleaseYear,
        string DurationToString,
        string CountryName,
        string CountryCode,
        string GenreName,
        string GenreDescription,
        string BoxOfficeToString,
        string BudgetToString,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        bool HasPoster,
        bool HasThumbnail,
        int GelleryImagesCount,
        Guid DirectorId,
        Guid StudioId
        );
}
