namespace Application.DTOs.Request.Movie
{
    public sealed record AwardItemRequest(
        int CategoryId,
        int InstitutionId,
        int Year
    );

    public sealed record AddAwardsToMovieRequest(
        List<AwardItemRequest> Awards
    );
}
