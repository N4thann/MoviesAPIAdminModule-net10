using Domain.Entities;
using Domain.SmartEnums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.EntitiesConfigurations
{
    public class MovieConfiguration : BaseEntityConfiguration<Movie>
    {
        protected override void AppendConfig(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movie");

            builder.Property(m => m.OriginalTitle)
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.Property(m => m.Synopsis)
                   .HasMaxLength(2000)
                   .IsRequired();

            builder.Property(m => m.ReleaseYear)
                   .IsRequired();

            builder.Property(d => d.IsActive)
                .IsRequired();

            builder.Property(m => m.CreatedAt)
                   .IsRequired();

            builder.Property(m => m.UpdatedAt)
                   .IsRequired();

            builder.Property(m => m.StudioId)
                   .IsRequired();

            builder.Property(m => m.DirectorId)
                   .IsRequired();

            builder.Ignore(m => m.Poster);
            builder.Ignore(m => m.Thumbnail);
            builder.Ignore(m => m.GalleryImages);
            builder.Ignore(m => m.HasPoster);
            builder.Ignore(m => m.HasThumbnail);
            builder.Ignore(m => m.GalleryImagesCount);

            // Owned Types
            builder.OwnsOne(m => m.Duration, durationBuilder =>
            {
                durationBuilder.Property(d => d.Minutes)
                               .HasColumnName("DurationInMinutes")
                               .IsRequired();
            });

            builder.OwnsOne(m => m.Country, countryBuilder =>
            {
                countryBuilder.Property(c => c.Name)
                              .HasColumnName("CountryName")
                              .HasMaxLength(100)
                              .IsRequired();

                countryBuilder.Property(c => c.Code)
                              .HasColumnName("CountryCode")
                              .HasMaxLength(3)
                              .IsRequired();
            });

            builder.OwnsOne(m => m.Rating, ratingBuilder =>
            {
                ratingBuilder.Property(r => r.TotalSum)
                             .HasColumnName("RatingTotalSum")
                             .HasColumnType("decimal(18,2)")
                             .IsRequired();

                ratingBuilder.Property(r => r.VotesCount)
                             .HasColumnName("RatingVotesCount")
                             .IsRequired();

                ratingBuilder.Property(r => r.MaxValue)
                             .HasColumnName("RatingMaxValue")
                             .HasColumnType("decimal(18,2)")
                             .IsRequired();
            });

            builder.OwnsOne(m => m.BoxOffice, moneyBuilder =>
            {
                moneyBuilder.Property(mo => mo.Amount)
                            .HasColumnName("BoxOfficeAmount")
                            .HasColumnType("decimal(18,2)");

                moneyBuilder.Property(mo => mo.Currency)
                            .HasColumnName("BoxOfficeCurrency")
                            .HasMaxLength(3);
            });

            builder.OwnsOne(m => m.Budget, moneyBuilder =>
            {
                moneyBuilder.Property(mo => mo.Amount)
                            .HasColumnName("BudgetAmount")
                            .HasColumnType("decimal(18,2)");

                moneyBuilder.Property(mo => mo.Currency)
                            .HasColumnName("BudgetCurrency")
                            .HasMaxLength(3);
            });

            builder.OwnsOne(m => m.Genre, genreBuilder =>
            {
                genreBuilder.Property(c => c.Name)
                              .HasColumnName("GenreName")
                              .HasMaxLength(50)
                              .IsRequired();

                genreBuilder.Property(c => c.Description)
                              .HasColumnName("GenreDescription")
                              .HasMaxLength(500)
                              .IsRequired();
            });

            // Owned Collections
            builder.OwnsMany(m => m.Awards, awardBuilder =>
            {
                awardBuilder.ToTable("MovieAwards");
                awardBuilder.WithOwner().HasForeignKey("MovieId");

                awardBuilder.HasKey(a => a.Id);

                awardBuilder.Property(a => a.Id).ValueGeneratedNever();

                // O mapeamento dos Smart Enums
                awardBuilder.Property(a => a.Category)
                    .HasColumnName("AwardCategoryId")
                    .IsRequired()
                    .HasConversion(
                        category => category.Id,
                        id => AwardCategory.FromValue<AwardCategory>(id)
                    );

                awardBuilder.Property(a => a.Institution)
                    .HasColumnName("InstitutionId")
                    .IsRequired()
                    .HasConversion(
                        institution => institution.Id,
                        id => Institution.FromValue<Institution>(id)
                    );

                awardBuilder.Property(a => a.Year).IsRequired();
            });


            builder.OwnsMany(m => m.Images, imageBuilder =>
            {
                imageBuilder.ToTable("MovieImages");
                imageBuilder.WithOwner().HasForeignKey("MovieId");
                imageBuilder.HasKey(i => i.Id);

                //especificando para que trate o Id como Id de identificação e não usar o do banco, ele é gerado no C#
                imageBuilder.Property(i => i.Id).ValueGeneratedNever();

                imageBuilder.Property(i => i.Url).IsRequired();
                imageBuilder.Property(i => i.AltText).HasMaxLength(200);
                imageBuilder.Property(i => i.Type)
                    .HasConversion<string>()
                    .IsRequired();
            });

            builder.HasOne(m => m.Director)
                   .WithMany()
                   .HasForeignKey(m => m.DirectorId)
                   .IsRequired();

            builder.HasOne(m => m.Studio)
                   .WithMany()
                   .HasForeignKey(m => m.StudioId)
                   .IsRequired();
        }
    }
}
