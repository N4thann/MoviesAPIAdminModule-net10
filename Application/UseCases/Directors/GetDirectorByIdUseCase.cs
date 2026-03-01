using Application.DTOs.Mappings;
using Application.DTOs.Response.Directors;
using Application.Interfaces.Mediator;
using Application.Queries.Director;
using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Mapster;

namespace Application.UseCases.Directors
{
    public class GetDirectorByIdUseCase : IQueryHandler<GetDirectorByIdQuery, Result<DirectorInfoResponse>>
    {
        private readonly IRepository<Director> _repository;

        public GetDirectorByIdUseCase(IRepository<Director> repository) => _repository = repository;

        public async Task<Result<DirectorInfoResponse>> Handle(GetDirectorByIdQuery query, CancellationToken cancellationToken)
        {
            var director = await _repository.GetByIdAsync(query.Id);

            if (director == null)
                return Result<DirectorInfoResponse>.AsFailure(Failure.NotFound("Director", query.Id));

            var response = director.Adapt<DirectorInfoResponse>();

            return Result<DirectorInfoResponse>.AsSuccess(response);
        }
    }
}
