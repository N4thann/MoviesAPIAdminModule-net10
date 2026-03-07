using Application.DTOs.Response.Studios;
using Application.Interfaces.Mediator;
using Application.Queries.Studio;
using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Mapster;

namespace Application.UseCases.Studios
{
    public class GetStudioByIdUseCase : IQueryHandler<GetStudioByIdQuery, Result<StudioInfoResponse>>
    {
        private readonly IRepository<Studio> _repository;

        public GetStudioByIdUseCase(IRepository<Studio> repository)  => _repository = repository;

        public async Task<Result<StudioInfoResponse>> Handle(GetStudioByIdQuery query, CancellationToken cancellationToken)
        {
            var studio = await _repository.GetByIdAsync(query.Id);

            if (studio == null)
                return Result<StudioInfoResponse>.AsFailure(Failure.NotFound("Studio", query.Id));

            var response = studio.Adapt<StudioInfoResponse>();

            return Result<StudioInfoResponse>.AsSuccess(response);
        }
    }
}
