using Application.DTOs.Response.Directors;
using Application.Interfaces.Mediator;
using Application.Queries.Director;
using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Mapster;
using Pandorax.PagedList;
using Pandorax.PagedList.EntityFrameworkCore;

namespace Application.UseCases.queryable
{
    public class DirectorFilterUseCase : IQueryHandler<DirectorFilterQuery, Result<IPagedList<DirectorTableResponse>>>
    {
        private readonly IRepository<Director> _repository;

        public DirectorFilterUseCase(IRepository<Director> repository) => _repository = repository;

        public async Task<Result<IPagedList<DirectorTableResponse>>> Handle(DirectorFilterQuery query, CancellationToken cancellationToken)
        {
            var queryable = _repository.GetAllQueryable();

            if (!string.IsNullOrEmpty(query.Name))
                queryable = queryable.Where(d => d.Name.Contains(query.Name));

            if (!string.IsNullOrEmpty(query.CountryName))
                queryable = queryable.Where(d => d.Country.Name.Contains(query.CountryName));

            if (query.Active.HasValue)
                queryable = queryable.Where(d => d.IsActive == query.Active.Value);

            if (query.AgeBegin.HasValue)
            {
                var minBirthDateForAge = DateTime.Today.AddYears(-query.AgeBegin.Value);
                queryable = queryable.Where(d => d.BirthDate <= minBirthDateForAge);
            }

            if (query.AgeEnd.HasValue)
            {
                var maxBirthDateForAge = DateTime.Today.AddYears(-(query.AgeEnd.Value + 1)); 
                queryable = queryable.Where(d => d.BirthDate >= maxBirthDateForAge);
            }

            var directorsPaged = await queryable
                .OrderBy(m => m.Name)
                .ProjectToType<DirectorTableResponse>()
                .ToPagedListAsync(query.Parameters.PageNumber, query.Parameters.PageSize, cancellationToken);

            return Result<IPagedList<DirectorTableResponse>>.AsSuccess(directorsPaged);
        }
    }
}
