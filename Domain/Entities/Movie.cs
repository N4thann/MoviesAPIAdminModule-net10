using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Domain.SeedWork.Validation;
using Domain.ValueObjects;
using MoviesAPIAdminModule.Domain.SeedWork;

namespace Domain.Entities
{
    public class Movie : BaseEntity, IAggregateRoot
    {
        private const int MIN_RELEASE_YEAR = 1888;
        private const int MAX_FUTURE_YEARS = 5;
        private const int MAX_TITLE_LENGTH = 100;
        private const int MIN_TITLE_LENGTH = 3;
        private const int MAX_SYNOPSIS_LENGTH = 2000;
        private const int MIN_SYNOPSIS_LENGTH = 10;
        private const int MAX_GALLERY_IMAGES = 4;

        private readonly List<Award> _awards;
        private readonly List<MovieImage> _images;

        protected Movie()
        {
            _awards = new List<Award>();
            _images = new List<MovieImage>();
        }

        private Movie(
            string title,
            string originalTitle,
            string synopsis,
            int releaseYear,
            Duration duration,
            Country country,
            Studio studio,
            Director director,
            Genre genre,
            Money? boxOffice,
            Money? budget) : this()
        {
            Name = title.Trim();
            OriginalTitle = originalTitle.Trim();
            Synopsis = synopsis.Trim();
            ReleaseYear = releaseYear;
            Duration = duration;
            Country = country;
            Studio = studio;
            StudioId = Studio.Id;
            BoxOffice = boxOffice;
            Budget = budget;
            Director = director;
            DirectorId = Director.Id;
            Genre = genre;
            Rating = Rating.CreateEmpty(10);
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public static Result<Movie> Create(string title,
            string originalTitle,
            string synopsis,
            int releaseYear,
            Duration duration,
            Country country,
            Studio studio,
            Director director,
            Genre genre,
            Money? boxOffice = null,
            Money? budget = null)
        {
            var maxYear = DateTime.UtcNow.Year + MAX_FUTURE_YEARS;

            var validationResult = Validate.NotNullOrEmpty(title, nameof(title))
                .Combine(
                Validate.NotNullOrEmpty(originalTitle, nameof(originalTitle)),
                Validate.MinLength(title, MIN_TITLE_LENGTH, nameof(title)),
                 Validate.MaxLength(title, MAX_TITLE_LENGTH, nameof(title)),
                Validate.MinLength(originalTitle, MIN_TITLE_LENGTH, nameof(originalTitle)),
                Validate.MaxLength(originalTitle, MAX_TITLE_LENGTH, nameof(originalTitle)),
                Validate.NotNullOrEmpty(synopsis, nameof(synopsis)),
                Validate.MinLength(synopsis, MIN_SYNOPSIS_LENGTH, nameof(synopsis)),
                Validate.MaxLength(synopsis, MAX_SYNOPSIS_LENGTH, nameof(synopsis)),
                Validate.Range(releaseYear, MIN_RELEASE_YEAR, maxYear, nameof(releaseYear)),
                Validate.NotNull(country, nameof(country)),
                Validate.NotNull(duration, nameof(duration)),
                Validate.NotNull(studio, nameof(studio)),
                Validate.NotNull(director, nameof(director)),
                Validate.NotNull(genre, nameof(genre)));

            if (validationResult.IsFailure)
                return Result<Movie>.AsFailure(validationResult.Failure!);

            var movie = new Movie(title, originalTitle, synopsis, releaseYear, duration,
                    country, studio, director, genre, boxOffice, budget);

            return Result<Movie>.AsSuccess(movie);
        }

        // Propriedades principais
        public string OriginalTitle { get; private set; }
        public string Synopsis { get; private set; }
        public int ReleaseYear { get; private set; }
        public Duration Duration { get; private set; }
        public Country Country { get; private set; }
        public Studio Studio { get; private set; }
        public Director Director { get; private set; }
        public Genre Genre { get; private set; }
        public Rating Rating { get; private set; }
        public Money? BoxOffice { get; private set; }
        public Money? Budget { get; private set; }

        // propriedades de chave estrangeira (FKs explícitas)
        public Guid DirectorId { get; private set; }
        public Guid StudioId { get; private set; }

        // Propriedades de controle
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        public IReadOnlyCollection<Award> Awards => _awards.AsReadOnly();
        public IReadOnlyCollection<MovieImage> Images => _images.AsReadOnly();

        // Propriedades calculadas para imagens
        public MovieImage? Poster => _images.FirstOrDefault(img => img.Type == MovieImage.ImageType.Poster);
        public MovieImage? Thumbnail => _images.FirstOrDefault(img => img.Type == MovieImage.ImageType.Thumbnail);
        public IEnumerable<MovieImage> GalleryImages => _images.Where(img => img.Type == MovieImage.ImageType.Gallery);

        // Outras propriedades calculadas
        public bool HasPoster => Poster! != null;
        public bool HasThumbnail => Thumbnail! != null;
        public int GalleryImagesCount => GalleryImages.Count();

        #region Métodos de Negócio - Informações Básicas

        public Result<bool> UpdateBasicInfo(string title, string originalTitle, string synopsis, Duration durationInMinutes, int releaseYear)
        {
            var maxYear = DateTime.UtcNow.Year + MAX_FUTURE_YEARS;

            var validationResult = Validate.NotNullOrEmpty(title, nameof(title))
                .Combine(
                    Validate.NotNullOrEmpty(originalTitle, nameof(originalTitle)),
                    Validate.MinLength(title, MIN_TITLE_LENGTH, nameof(title)),
                    Validate.MaxLength(title, MAX_TITLE_LENGTH, nameof(title)),
                    Validate.MinLength(originalTitle, MIN_TITLE_LENGTH, nameof(originalTitle)),
                    Validate.MaxLength(originalTitle, MAX_TITLE_LENGTH, nameof(originalTitle)),
                    Validate.Range(releaseYear, MIN_RELEASE_YEAR, maxYear, nameof(releaseYear)),
                    Validate.NotNull(durationInMinutes, nameof(durationInMinutes)),
                    Validate.NotNullOrEmpty(synopsis, nameof(synopsis)),
                    Validate.MinLength(synopsis, MIN_SYNOPSIS_LENGTH, nameof(synopsis)),
                    Validate.MaxLength(synopsis, MAX_SYNOPSIS_LENGTH, nameof(synopsis))
                    );

            if (validationResult.IsFailure)
                return Result<bool>.AsFailure(validationResult.Failure!);

            Name = title.Trim();
            OriginalTitle = string.IsNullOrWhiteSpace(originalTitle) ? title.Trim() : originalTitle.Trim();
            Synopsis = synopsis.Trim();
            Duration = durationInMinutes;
            ReleaseYear = releaseYear;
            UpdatedAt = DateTime.UtcNow;

            return Result<bool>.AsSuccess(true);
        }

        public void UpdateProductionInfo(Money budget, Money boxOffice)
        {
            Validate.NotNull(budget, nameof(budget));
            Validate.NotNull(boxOffice, nameof(boxOffice));

            Budget = budget;
            BoxOffice = boxOffice;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateGenreInfo(Genre genre)
        {
            Validate.NotNull(genre, nameof(genre));

            Genre = genre;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDirectorAndStudioInfo(Guid directorId, Guid studioId)
        {
            StudioId = studioId;
            DirectorId = directorId;
            UpdatedAt = DateTime.UtcNow;
        }

        public Result<bool> UpdateCountry(Country country)
        {
            var validationResult = Validate.NotNull(country, nameof(country));
            if (validationResult.IsFailure)
                return Result<bool>.AsFailure(validationResult.Failure!);

            Country = country;
            UpdatedAt = DateTime.UtcNow;

            return Result<bool>.AsSuccess(true);
        }

        #endregion

        #region Métodos de Negócio - Prêmios

        public Result<bool> AddAward(Award award)
        {
            var validation = Validate.NotNull(award, nameof(award));

            if (validation.IsFailure)
                return Result<bool>.AsFailure(validation.Failure!);

            if (_awards.Contains(award))
                return Result<bool>.AsFailure(Failure.Conflict("Filme já possui este prêmio neste ano"));

            _awards.Add(award);
            UpdatedAt = DateTime.UtcNow;

            return Result<bool>.AsSuccess(true);
        }

        public Result<bool> RemoveAward(Award award)
        {
            var validate = Validate.NotNull(award, nameof(award));

            if (validate.IsFailure) 
                return Result<bool>.AsFailure(validate.Failure!);

            if (!_awards.Remove(award))
                return Result<bool>.AsFailure(Failure.Validation("Prêmio não encontrado para este filme."));

            UpdatedAt = DateTime.UtcNow;

            return Result<bool>.AsSuccess(true);
        }
        #endregion

        #region Métodos de Negócio - Imagens

        public Result<bool> SetPoster(MovieImage poster)
        {
            var validate = Validate.NotNull(poster, nameof(poster));

            if (validate.IsFailure)
                return Result<bool>.AsFailure(validate.Failure!);

            if (poster.Type != MovieImage.ImageType.Poster)
                return Result<bool>.AsFailure(Failure.Validation("Imagem deve ser do tipo Poster"));

            var existingPoster = Poster;

            if (existingPoster != null)
                _images.Remove(existingPoster);

            _images.Add(poster);
            UpdatedAt = DateTime.UtcNow;

            return Result<bool>.AsSuccess(true);
        }

        public Result<bool> SetThumbnail(MovieImage thumbnail)
        {
            Validate.NotNull(thumbnail, nameof(thumbnail));

            if (thumbnail.Type != MovieImage.ImageType.Thumbnail)
                return Result<bool>.AsFailure(Failure.Validation("Imagem deve ser do tipo Thumbnail"));

            var existingThumbnail = Thumbnail;

            if (existingThumbnail != null)
                _images.Remove(existingThumbnail);

            _images.Add(thumbnail);
            UpdatedAt = DateTime.UtcNow;

            return Result<bool>.AsSuccess(true);
        }

        public Result<bool> AddGalleryImage(MovieImage galleryImage)
        {
            var validation = Validate.NotNull(galleryImage, nameof(galleryImage));

            if (validation.IsFailure)
                return Result<bool>.AsFailure(validation.Failure!);

            if (galleryImage.Type != MovieImage.ImageType.Gallery)
                return Result<bool>.AsFailure(Failure.Validation("Image must be of type Gallery."));

            if (GalleryImagesCount >= MAX_GALLERY_IMAGES)
                return Result<bool>.AsFailure(Failure.Conflict($"Maximum of {MAX_GALLERY_IMAGES} gallery images allowed."));

            _images.Add(galleryImage);
            UpdatedAt = DateTime.UtcNow;

            return Result<bool>.AsSuccess(true);
        }

        public Result<bool> RemoveImage(Guid imageId)
        {
            var imageToRemove = _images.FirstOrDefault(i => i.Id == imageId);

            if (imageToRemove is null)
                return Result<bool>.AsFailure(Failure.NotFound("Image", imageId));

            _images.Remove(imageToRemove);
            UpdatedAt = DateTime.UtcNow;

            return Result<bool>.AsSuccess(true);
        }

        #endregion


        #region Métodos de Negócio - Regras Calculadas

        public bool IsBlockbuster() => BoxOffice?.Amount >= 100_000_000; 

        public bool IsWellRated(decimal threshold = 7.0m)
        {
            Validate.Range((int)threshold, 1, 10, nameof(threshold));
            return Rating.HasVotes && Rating.AverageValue >= threshold;
        }


        public bool IsClassic() => DateTime.UtcNow.Year - ReleaseYear >= 30;
        

        public bool IsRecent() => DateTime.UtcNow.Year - ReleaseYear <= 3;

        public bool WasProfitable() => 
                   Budget != null && BoxOffice != null &&
                   BoxOffice.Currency == Budget.Currency &&
                   BoxOffice.Amount > Budget.Amount;

        public bool IsLongMovie() => Duration.Minutes > 150;

        public override string ToString() => $"{Name} ({ReleaseYear}) - {Duration.ToString} - {Rating}";
        #endregion
    }
}
