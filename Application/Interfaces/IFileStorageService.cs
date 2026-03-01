using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.ValueObjects;

namespace Application.Interfaces
{
    /// <summary>
    /// Contrato para um serviço responsável por armazenar arquivos (por exemplo imagens de filmes).
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Salva um arquivo de forma assíncrona e retorna o caminho/identificador resultante.
        /// </summary>
        /// <param name="fileStream">Stream contendo os dados do arquivo.</param>
        /// <param name="originalFileName">Nome original do arquivo enviado.</param>
        /// <param name="contentType">Tipo de conteúdo (MIME) do arquivo.</param>
        /// <param name="movie">O filme ao qual o arquivo está associado.</param>
        /// <param name="imageType">O tipo de imagem (Poster, Thumbnail, Gallery, etc.).</param>
        /// <returns>Um <see cref="Result{String}"/> contendo o caminho/identificador do arquivo em caso de sucesso, ou uma falha.</returns>
        Task<Result<string>> SaveFileAsync(Stream fileStream, string originalFileName, string contentType, Movie movie, MovieImage.ImageType imageType);
    }
}