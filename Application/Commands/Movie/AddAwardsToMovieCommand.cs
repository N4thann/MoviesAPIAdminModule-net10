using Application.DTOs.Request.Movie;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;

namespace Application.Commands.Movie
{
    public record class AddAwardsToMovieCommand(
        Guid MovieId,
        List<AwardItemRequest> Awards
    ) : ICommand<Result<bool>>;
}
