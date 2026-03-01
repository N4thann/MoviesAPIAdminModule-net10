using Application.DTOs.Response.Movies;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;

namespace Application.Commands.Movie
{
    public record class CreateMovieCommand(
        string Title,
        string OriginalTitle,
        string Synopsis,
        int ReleaseYear,
        int DurationMinutes,
        string CountryName,
        string CountryCode,
        string GenreName,
        string GenreDescription,
        decimal BoxOfficeAmount,
        string BoxOfficeCurrency,
        decimal BudgetAmount,
        string BudgetCurrency,
        Guid DirectorId,
        Guid StudioId
        ) : ICommand<Result<MovieBasicInfoResponse>>;
}
