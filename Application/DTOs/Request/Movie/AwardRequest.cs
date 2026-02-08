namespace Application.DTOs.Request.Movie
{
    public sealed record AwardRequest(
         int CategoryId,   
         int InstitutionId,
         int Year
    );
}
