namespace Application.DTOs.Response.Movies
{
    public sealed record MovieTableResponse(
        Guid Id,
        string Name,
        string OriginalTitle,
        int ReleaseYear,
        string CountryName,
        string GenreName,
        bool IsActive
     );
}
