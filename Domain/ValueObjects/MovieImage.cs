using Domain.SeedWork;
using Domain.SeedWork.Core;
using Domain.SeedWork.Validation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace Domain.ValueObjects
{
    public class MovieImage : ValueObject
    {
        public MovieImage() { }

        private MovieImage(string url, string altText, ImageType type)
        {
            Id = Guid.NewGuid();
            Url = url;
            AltText = altText;
            Type = type;
        }

        public static Result<MovieImage> Create(string url, string altText = null, ImageType type = ImageType.Gallery)
        {
            var validation1 = Validate.NotNullOrEmpty(url, nameof(url));
            var validation2 = Validate.IsValidUrl(url, nameof(url));

            if (validation1.IsFailure)
                return Result<MovieImage>.AsFailure(validation1.Failure!);
            if (validation2.IsFailure)
                return Result<MovieImage>.AsFailure(validation2.Failure!);

            if (!string.IsNullOrEmpty(altText))
            {
                var validation3 = Validate.MaxLength(altText, 200, nameof(altText));

                if (validation3.IsFailure)
                    return Result<MovieImage>.AsFailure(validation3.Failure!);
            }

            var movieImage = new MovieImage(url, altText, type);
            return Result<MovieImage>.AsSuccess(movieImage);
        }

        public Guid Id { get; private set; }
        public string Url { get; private set; }
        public string AltText { get; private set; }
        public ImageType Type { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Url;
            yield return Type;
        }

        public override string ToString() => $"{Type}: {Url}";

        public enum ImageType
        {
            Poster = 1,
            Gallery = 2,
            Thumbnail = 3
        }
    }
}
