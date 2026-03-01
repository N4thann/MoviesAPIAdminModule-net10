using Application.DTOs.Response.Movies;
using Application.Interfaces.Mediator;
using Application.Queries.Movie;
using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Mapster;
using Pandorax.PagedList;
using Pandorax.PagedList.EntityFrameworkCore;

namespace Application.UseCases.Movies
{
    public class MovieBasicFilterUseCase : IQueryHandler<MovieBasicFilterQuery, Result<IPagedList<MovieTableResponse>>>
    {
        private readonly IRepository<Movie> _repository;

        public MovieBasicFilterUseCase(IRepository<Movie> repository) => _repository = repository;

        public async Task<Result<IPagedList<MovieTableResponse>>> Handle(MovieBasicFilterQuery query, CancellationToken cancellationToken)
        {
            var queryable = _repository.GetAllQueryable();

            if (!string.IsNullOrEmpty(query.Title))
                queryable = queryable.Where(m => m.Name.Contains(query.Title));

            if (!string.IsNullOrEmpty(query.OriginalTitle))
                queryable = queryable.Where(m => m.OriginalTitle.Contains(query.OriginalTitle));

            if (!string.IsNullOrEmpty(query.CountryName))
                queryable = queryable.Where(m => m.Country.Name.Contains(query.CountryName));

            if (query.Active.HasValue)
                queryable = queryable.Where(d => d.IsActive == query.Active.Value);

            if (query.ReleaseYearBegin.HasValue)
                queryable = queryable.Where(m => m.ReleaseYear >= query.ReleaseYearBegin.Value);

            if (query.ReleaseYearEnd.HasValue)
                queryable = queryable.Where(m => m.ReleaseYear <= query.ReleaseYearEnd.Value);

            var moviesPaged = await queryable
                .OrderBy(m => m.Name)
                .ProjectToType<MovieTableResponse>()
                .ToPagedListAsync(query.Parameters.PageNumber, query.Parameters.PageSize, cancellationToken);

            return Result<IPagedList<MovieTableResponse>>.AsSuccess(moviesPaged);
        }
    }
}
