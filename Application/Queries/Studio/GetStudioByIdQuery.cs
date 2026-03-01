using Application.DTOs.Response.Studios;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;

namespace Application.Queries.Studio
{
    public record class GetStudioByIdQuery(
        Guid Id
        ) : IQuery<Result<StudioInfoResponse>>;
}
