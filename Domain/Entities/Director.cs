using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Domain.SeedWork.Validation;
using Domain.ValueObjects;
using MoviesAPIAdminModule.Domain.SeedWork;

namespace Domain.Entities
{
    public class Director : BaseEntity, IAggregateRoot
    {
        private const int MAX_BIO_LENGTH = 1000;

        protected Director() { }

        private Director(
            string name,
            DateTime birthDate,
            Country country,
            string? biography,
            Gender gender) : this() 
        {
            Name = name.Trim();
            BirthDate = birthDate.Date;
            Country = country;
            Biography = biography?.Trim();
            Gender = gender;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public static Result<Director> Create(string name,
            DateTime birthDate,
            Country country,
            string? biography = null,
            Gender gender = Gender.NotSpecified)
        {
            var validationResult = Validate.NotNullOrEmpty(name, nameof(name))
                .Combine(
                Validate.MaxLength(name, 50, nameof(name)),
                Validate.IsPastDate(birthDate, nameof(birthDate), allowToday: false),
                Validate.NotNull(country, nameof(country))
                );

            if (validationResult.IsSuccess && !string.IsNullOrWhiteSpace(biography))
                validationResult = validationResult.Combine(Validate.MaxLength(biography, MAX_BIO_LENGTH, nameof(biography)));

            if (validationResult.IsFailure)
                return Result<Director>.AsFailure(validationResult.Failure!);

            var director = new Director(name, birthDate, country, biography, gender);

            return Result<Director>.AsSuccess(director);
        }

        // Propriedades específicas do Diretor
        public DateTime BirthDate { get; private set; }
        public Country Country { get; private set; } 
        public string? Biography { get; private set; }
        public Gender Gender { get; private set; }

        // Propriedades de controle
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Propriedades calculadas
        public int Age => CalculateAge(BirthDate);

        #region Métodos de Negócio - Informações Básicas
        public Result<bool> UpdateBasicInfo(string name, DateTime newBirthDate, Gender gender = Gender.NotSpecified, string? biography = null)
        {
            var validationResult = Validate.NotNullOrEmpty(name, nameof(name))
                .Combine(
                    Validate.MaxLength(name, 50, nameof(name)),
                    Validate.IsPastDate(newBirthDate, nameof(newBirthDate), allowToday: false),
                    Validate.IsDefinedEnum(gender, nameof(gender)) // <-- Nova validação adicionada!
                );

            if (validationResult.IsSuccess && !string.IsNullOrWhiteSpace(biography))
                validationResult = validationResult.Combine(Validate.MaxLength(biography, MAX_BIO_LENGTH, nameof(biography)));

            if (validationResult.IsFailure)
                return Result<bool>.AsFailure(validationResult.Failure!);

            Name = name.Trim();
            Biography = biography?.Trim();
            BirthDate = newBirthDate.Date;
            Gender = gender;
            UpdatedAt = DateTime.UtcNow;

            return Result<bool>.AsSuccess(true);
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

        #region Métodos de Negócio - Regras Calculadas
        private static int CalculateAge(DateTime birthDate)
        {
            var today = DateTime.Today;
            var age = today.Year - birthDate.Year;

            if (birthDate.Date > today.AddYears(-age)) 
                age--;

            return age;
        }

        public override string ToString() => $"{Name} ({Age} anos) - {Country.Name}";

        #endregion
    }

    public enum Gender
    {
        NotSpecified = 0,
        Male = 1,
        Female = 2,
        Other = 3
    }
}