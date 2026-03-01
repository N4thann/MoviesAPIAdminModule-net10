using Application.Commands.Studio;
using Application.DTOs.Response.Studios;
using Application.Interfaces.Mediator;
using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Domain.ValueObjects;
using Mapster;

namespace Application.UseCases.Studios
{
    public class CreateStudioUseCase : ICommandHandler<CreateStudioCommand, Result<StudioInfoResponse>>
    {
        private readonly IRepository<Studio> _repository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateStudioUseCase(IRepository<Studio> repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<StudioInfoResponse>> Handle(CreateStudioCommand command, CancellationToken cancellationToken)
        {
            var countryResult = Country.Create(command.CountryName, command.CountryCode);

            if (countryResult.IsFailure)
                return Result<StudioInfoResponse>.AsFailure(countryResult.Failure!);

            var studioResult = Studio.Create(
                command.Name,
                countryResult.Success!,
                command.FoundationDate,
                command.History
                );

            if (studioResult.IsFailure)
                return Result<StudioInfoResponse>.AsFailure(studioResult.Failure!);

            var studio = studioResult.Success!;

            _repository.Add(studio);
            await _unitOfWork.Commit(cancellationToken);

            var response = studio.Adapt<StudioInfoResponse>();

            return Result<StudioInfoResponse>.AsSuccess(response);
        }
    }
}
