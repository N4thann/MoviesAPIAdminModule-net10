using Application.DTOs.Response;
using Application.Interfaces.Mediator;
using Application.Queries.Dashboard;
using Application.Queries.User;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MoviesAPIAdminModule.Filters;
using NSwag.Annotations;

namespace MoviesAPIAdminModule.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ServiceFilter(typeof(ApiLoggingFilter))]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [OpenApiTag("Dashboard")]
    public class DashboardController : BaseApiController
    {
        private readonly IMediator _mediator;

        public DashboardController(IMediator mediator) => _mediator = mediator;

        //public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
        //{
        //    var query = new DashboardQuery();
        //    var result = await _mediator.Query<DashboardQuery, Results<DashboardResponse>>(query, cancellationToken);

        //    if (result.IsFailure)
        //        return HandleFailure(result.Failure!);

        //    return Ok(result.Success);
        //}
    }
}
