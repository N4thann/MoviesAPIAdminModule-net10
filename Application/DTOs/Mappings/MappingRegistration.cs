using Application.DTOs.Response.Directors;
using Application.DTOs.Response.Movies;
using Application.DTOs.Response.Studios;
using Domain.Entities;
using Domain.ValueObjects;
using Mapster;

namespace Application.DTOs.Mappings
{
    public class MappingRegistration : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<Movie, MovieTableResponse>()
                .Map(dest => dest.CountryName, src => src.Country.Name)
                .Map(dest => dest.GenreName, src => src.Genre.Name);

            config.NewConfig<Movie, MovieBasicInfoResponse>()
                .Map(dest => dest.DurationToString, src => src.Duration.ToString())
                .Map(dest => dest.CountryName, src => src.Country.Name)
                .Map(dest => dest.CountryCode, src => src.Country.Code)
                .Map(dest => dest.GenreName, src => src.Genre.Name)
                .Map(dest => dest.GenreDescription, src => src.Genre.Description)
                .Map(dest => dest.BoxOfficeToString, src => src.BoxOffice != null ? src.BoxOffice.ToString() : "N/A")
                .Map(dest => dest.BudgetToString, src => src.Budget != null ? src.Budget.ToString() : "N/A")
                .Map(dest => dest.GelleryImagesCount, src => src.Images.Count(i => i.Type == MovieImage.ImageType.Gallery));

            config.NewConfig<Studio, StudioInfoResponse>()
                .Map(dest => dest.CountryName, src => src.Country.Name)
                .Map(dest => dest.CountryCode, src => src.Country.Code);

            config.NewConfig<Studio, StudioTableResponse>()
                .Map(dest => dest.CountryName, src => src.Country.Name);

            config.NewConfig<Director, DirectorTableResponse>()
                .Map(dest => dest.CountryName, src => src.Country.Name)
                .Map(dest => dest.Age, src => src.Age);

            config.NewConfig<Director, DirectorInfoResponse>()
                .Map(dest => dest.CountryName, src => src.Country.Name)
                .Map(dest => dest.CountryCode, src => src.Country.Code)
                .Map(dest => dest.Age, src => src.Age);

            config.NewConfig<Director, DirectorDetailsResponse>()
                .Map(dest => dest.CountryName, src => src.Country.Name)
                .Map(dest => dest.Movies, src => new List<MovieSummaryResponse>()); 

            config.NewConfig<Award, AwardResponse>()
                .Map(dest => dest.Category, src => src.Category.Name)
                .Map(dest => dest.Institution, src => src.Institution.Name);
        }
    }
}
