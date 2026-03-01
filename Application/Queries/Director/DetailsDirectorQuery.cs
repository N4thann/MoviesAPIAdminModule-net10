using Application.DTOs.Response.Directors;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;

namespace Application.Queries.Director
{
    public record DetailsDirectorQuery(Guid Id) : IQuery<Result<DirectorDetailsResponse>>;
}
