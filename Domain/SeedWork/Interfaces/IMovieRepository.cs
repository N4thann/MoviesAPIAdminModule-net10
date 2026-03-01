using Domain.Entities;

namespace Domain.SeedWork.Interfaces
{
    public interface IMovieRepository
    {
        /// <summary>
        /// Obtém um filme pelo seu ID, incluindo a coleção de imagens relacionada.
        /// </summary>
        /// <param name="movieId">O identificador único do filme.</param>
        /// <returns>O aggregate root Movie com suas imagens, ou null caso não seja encontrado.</returns>
        Task<Movie?> GetByIdWithImagesAsync(Guid movieId);

        /// <summary>
        /// Obtém um filme pelo seu ID, incluindo a coleção de premiações relacionada.
        /// </summary>
        /// <param name="movieId">O identificador único do filme.</param>
        /// <returns>O aggregate root Movie com suas premiações, ou null caso não seja encontrado.</returns>
        Task<Movie?> GetByIdWithAwardAsync(Guid movieId);

        /// <summary>
        /// Recupera todos os filmes associados a um diretor específico, incluindo prêmios e gêneros.
        /// </summary>
        /// <param name="directorId">O identificador único do diretor.</param>
        /// <returns>Uma coleção de filmes dirigidos pelo diretor informado.</returns>
        Task<IEnumerable<Movie>> GetMoviesByDirectorIdAsync(Guid directorId);
    }
}
