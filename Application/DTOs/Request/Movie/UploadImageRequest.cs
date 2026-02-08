using Microsoft.AspNetCore.Http;
using static Domain.ValueObjects.MovieImage;

namespace Application.DTOs.Request.Movie
{
    public sealed record UploadImageRequest(
        IFormFile ImageFile,
        ImageType ImageType,
        string? AltText
        );
}
