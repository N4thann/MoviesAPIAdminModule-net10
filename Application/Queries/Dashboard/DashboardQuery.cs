using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Domain.SeedWork.Core;

namespace Application.Queries.Dashboard
{
    public record DashboardQuery() : IQuery<Result<DashboardResponse>>;
}
