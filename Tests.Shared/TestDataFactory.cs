using Domain.Entities;
using Domain.SeedWork.Core;
using Domain.SmartEnums;
using Domain.ValueObjects;

namespace Tests.Shared
{
    /// <summary>
    /// Provides factory methods for creating predefined and fully populated test data instances of directors, studios,
    /// and movies for use in unit tests, demonstrations, or reference scenarios.
    /// </summary>
    /// <remarks>This class includes methods to generate canonical representations of well-known entities such
    /// as Christopher Nolan, Warner Bros. Pictures, Legendary Pictures, and movies like 'Inception' and 'The Dark
    /// Knight'. All returned objects are constructed with realistic metadata and relationships, and are wrapped in
    /// result types to indicate success or failure. The methods are intended to simplify the setup of test cases and
    /// sample data by providing ready-to-use, consistent objects.</remarks>
    public static class TestDataFactory
    {
        #region === Entidades de Apoio (Diretores e Estúdios) ===

        /// <summary>
        /// Creates a predefined <see cref="Director"/> instance representing Christopher Nolan.    
        /// </summary>
        /// <remarks>The returned director includes name, birth date, and country information. This method
        /// is intended for scenarios where a standard representation of Christopher Nolan is required, such as test
        /// data or reference lists.</remarks>
        /// <returns>A <see cref="Result{Director}"/> containing the Christopher Nolan director instance if creation succeeds;
        /// otherwise, a failure result describing the error.</returns>
        public static Result<Director> ChristopherNolan()
        {
            var countryResult = Country.Create("United Kingdom", "UK");
            if (countryResult.IsFailure)
                return Result<Director>.AsFailure(countryResult.Failure!);

            return Director.Create(
                name: "Christopher Nolan",
                birthDate: new DateTime(1970, 7, 30),
                country: countryResult.Success!
            );
        }

        /// <summary>
        /// Creates a <see cref="Studio"/> instance representing Warner Bros. Pictures.
        /// </summary>
        /// <remarks>The returned studio is initialized with the name "Warner Bros. Pictures", a
        /// foundation date of April 4, 1923, and country information for the United States. If the country information
        /// cannot be created, the result will indicate failure.</remarks>
        /// <returns>A <see cref="Result{Studio}"/> containing the Warner Bros. Pictures studio if creation succeeds; otherwise,
        /// a failure result describing the error.</returns>
        public static Result<Studio> WarnerBros()
        {
            var countryResult = Country.Create("United States", "USA");
            if (countryResult.IsFailure)
                return Result<Studio>.AsFailure(countryResult.Failure!);

            return Studio.Create(
                name: "Warner Bros. Pictures",
                foundationDate: new DateTime(1923, 4, 4),
                country: countryResult.Success!
            );
        }

        /// <summary>
        /// Creates a predefined representation of the Legendary Pictures film studio.
        /// </summary>
        /// <remarks>The returned studio includes the name "Legendary Pictures", a foundation date of
        /// January 1, 2000, and is associated with the United States as its country.</remarks>
        /// <returns>A <see cref="Result{Studio}"/> containing the Legendary Pictures studio if creation succeeds; otherwise, a
        /// failure result describing the error.</returns>
        public static Result<Studio> LegendaryPictures()
        {
            var countryResult = Country.Create("United States", "USA");
            if (countryResult.IsFailure)
                return Result<Studio>.AsFailure(countryResult.Failure!);

            return Studio.Create(
                name: "Legendary Pictures",
                foundationDate: new DateTime(2000, 1, 1),
                country: countryResult.Success!
            );
        }

        #endregion

        #region === Fábricas de Filmes Completos ===

        /// <summary>
        /// Creates a fully populated <see cref="Movie"/> instance representing the film 'Inception', including
        /// director, studio, genre, awards, and poster image.
        /// </summary>
        /// <remarks>The returned movie includes metadata such as box office, budget, country, and an
        /// Academy Award for Best Original Screenplay. The poster image is set to a local URL. This method is intended
        /// for scenarios where a complete, canonical representation of 'Inception' is required for testing or
        /// demonstration purposes.</remarks>
        /// <returns>A <see cref="Result{Movie}"/> containing the completed 'Inception' movie. If creation fails due to invalid
        /// data, the result will indicate failure.</returns>
        public static Result<Movie> CreateInceptionMovie()
        {
            var director = ChristopherNolan().Success;
            var studio = WarnerBros().Success;

            var durationResult = Duration.Create(148);
            var countryResult = Country.Create("United States", "USA");
            var genreResult = Genre.Create("Science Fiction", "A sci-fi action thriller.");
            var boxOfficeResult = Money.Create(829900000, "USD");
            var budgetResult = Money.Create(160000000, "USD");

            var movieResult = Movie.Create(
                title: "A Origem",
                originalTitle: "Inception",
                synopsis: "Um ladrão que rouba segredos corporativos através do uso da tecnologia de compartilhamento de sonhos recebe a tarefa inversa de plantar uma ideia na mente de um C.E.O.",
                releaseYear: 2010,
                duration: durationResult.Success!,
                country: countryResult.Success!,
                studio: studio!,
                director: director!,
                genre: genreResult.Success!,
                boxOffice: boxOfficeResult.Success,
                budget: budgetResult.Success
            );

            if (movieResult.IsFailure) return movieResult;

            var movie = movieResult.Success!;

            var awardResult = Award.Create(AwardCategory.BestOriginalScreenplay, Institution.AcademyAwards, 2011);
            if (awardResult.IsSuccess)
                movie.AddAward(awardResult.Success!);

            var imageUrl = $"https://localhost:7211/StaticFiles/Images/inception-{movie.Id}/inception_2010_poster.jpg";
            var imageResult = MovieImage.Create(imageUrl, "Official movie poster", MovieImage.ImageType.Poster);
            if (imageResult.IsSuccess)
                movie.SetPoster(imageResult.Success!);

            return movieResult; 
        }

        /// <summary>
        /// Creates a fully populated <see cref="Movie"/> instance representing "The Dark Knight" (2008), including
        /// director, studio, genre, country, financials, awards, and poster image.
        /// </summary>
        /// <remarks>The returned movie includes the Academy Award for Best Supporting Actor (2009) and an
        /// official poster image if those components are successfully created. All movie details are pre-filled and
        /// localized as appropriate.</remarks>
        /// <returns>A <see cref="Result{Movie}"/> containing the created movie if successful; otherwise, a failure result
        /// describing the error.</returns>
        public static Result<Movie> CreateTheDarkKnightMovie()
        {
            var director = ChristopherNolan().Success;
            var studio = LegendaryPictures().Success;

            var durationResult = Duration.Create(152);
            var countryResult = Country.Create("United States", "USA");
            var genreResult = Genre.Create("Action", "A superhero thriller.");
            var boxOfficeResult = Money.Create(1006000000, "USD");
            var budgetResult = Money.Create(185000000, "USD");

            var movieResult = Movie.Create(
                title: "O Cavaleiro das Trevas",
                originalTitle: "The Dark Knight",
                synopsis: "Quando a ameaça conhecida como O Coringa emerge de seu passado misterioso, ele causa estragos e caos nas pessoas de Gotham.",
                releaseYear: 2008,
                duration: durationResult.Success!,
                country: countryResult.Success!,
                studio: studio!,
                director: director!,
                genre: genreResult.Success!,
                boxOffice: boxOfficeResult.Success,
                budget: budgetResult.Success
            );

            if (movieResult.IsFailure) 
                return movieResult;

            var movie = movieResult.Success!;

            var awardResult = Award.Create(AwardCategory.BestSupportingActor, Institution.AcademyAwards, 2009);

            if (awardResult.IsSuccess)
                movie.AddAward(awardResult.Success!);

            var imageUrl = $"https://localhost:7211/StaticFiles/Images/the-dark-knight-{movie.Id}/the-dark-knight_2008_poster.jpg";
            var imageResult = MovieImage.Create(imageUrl, "Official movie poster for The Dark Knight", MovieImage.ImageType.Poster);

            if (imageResult.IsSuccess)
                movie.SetPoster(imageResult.Success!);

            return movieResult;
        }
        #endregion
    }
}
