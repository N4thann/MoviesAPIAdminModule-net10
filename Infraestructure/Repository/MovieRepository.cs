using Domain.Entities;
using Domain.SeedWork.Interfaces;
using Infraestructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ApplicationDbContext _context;

        // Passamos o context para a classe base para que os métodos genéricos funcionem
        public MovieRepository(ApplicationDbContext context) : base(context) => _context = context;

        public async Task<Movie?> GetByIdWithImagesAsync(Guid movieId)
        {
            return await _context.Set<Movie>()
                                 .Include(m => m.Images)
                                 .FirstOrDefaultAsync(m => m.Id == movieId);
        }

        public async Task<Movie?> GetByIdWithAwardAsync(Guid movieId)
        {
            return await _context.Set<Movie>()
                                 .Include(m => m.Awards)
                                 .FirstOrDefaultAsync(m => m.Id == movieId);
        }

        public async Task<IEnumerable<Movie>> GetMoviesByDirectorIdAsync(Guid directorId)
        {
            return await _context.Set<Movie>()
                            .Include(m => m.Awards)
                            .Where(m => m.DirectorId == directorId)
                            .OrderByDescending(m => m.ReleaseYear)
                            .ToListAsync();
        }
    }
}
