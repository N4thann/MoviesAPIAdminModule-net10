using Application.Common;
using Application.DTOs.Mappings;
using Application.DTOs.Response.Directors;
using Application.DTOs.Response.Studios;
using Application.Interfaces.Mediator;
using Application.Queries.Studio;
using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Mapster;
using Pandorax.PagedList;
using Pandorax.PagedList.EntityFrameworkCore;
using System.Linq;

namespace Application.UseCases.Studios
{
    public class StudioFilterUseCase : IQueryHandler<StudioFilterQuery, Result<IPagedList<StudioTableResponse>>>
    {
        private readonly IRepository<Studio> _repository;

        public StudioFilterUseCase(IRepository<Studio> repository) => _repository = repository;

        public async Task<Result<IPagedList<StudioTableResponse>>> Handle(StudioFilterQuery query, CancellationToken cancellationToken)
        {
            var queryable = _repository.GetAllQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
                queryable = queryable.Where(s => s.Name.Contains(query.Name));

            if (!string.IsNullOrWhiteSpace(query.CountryName))
                queryable = queryable.Where(s => s.Country.Name.Contains(query.CountryName));

            if (query.Active.HasValue)
                queryable = queryable.Where(d => d.IsActive == query.Active.Value);

            if (query.FoundationYearBegin.HasValue)
                queryable = queryable.Where(s => s.FoundationDate.Year >= query.FoundationYearBegin.Value);

            if (query.FoundationYearEnd.HasValue)
                queryable = queryable.Where(s => s.FoundationDate.Year <= query.FoundationYearEnd.Value);

            var studioPaged = await queryable
                .OrderBy(m => m.Name)
                .ProjectToType<StudioTableResponse>()
                .ToPagedListAsync(query.Parameters.PageNumber, query.Parameters.PageSize, cancellationToken);

            return Result<IPagedList<StudioTableResponse>>.AsSuccess(studioPaged);
        }
    }
}
