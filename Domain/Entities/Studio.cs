using Domain.SeedWork.Core;
using Domain.SeedWork.Interfaces;
using Domain.SeedWork.Validation;
using Domain.ValueObjects;
using MoviesAPIAdminModule.Domain.SeedWork;

namespace Domain.Entities
{
    public class Studio : BaseEntity, IAggregateRoot
    {
        private const int MAX_HISTORY_LENGTH = 5000;
        private const int MAX_NAME_LENGTH = 100;

        protected Studio() { }

        private Studio(string name, Country country, DateTime foundationDate, string? history = null)
        {
            Name = name.Trim();
            Country = country;
            FoundationDate = foundationDate.Date;
            History = history?.Trim();
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public static Result<Studio> Create(string name, Country country, DateTime foundationDate, string? history = null)
        {
            var validationResult = Validate.NotNullOrEmpty(name, nameof(name))
                .Combine(
                    Validate.MaxLength(name, MAX_NAME_LENGTH, nameof(name)),
                    Validate.NotNull(country, nameof(country)),
                    Validate.IsPastDate(foundationDate, nameof(foundationDate), allowToday: true)
                );

            if (validationResult.IsFailure)
                return Result<Studio>.AsFailure(validationResult.Failure!);

            if (!string.IsNullOrWhiteSpace(history))
            {
                var historyValidation = Validate.MaxLength(history, MAX_HISTORY_LENGTH, nameof(history));
                if (historyValidation.IsFailure)
                {
                    return Result<Studio>.AsFailure(historyValidation.Failure!);
                }
            }

            var studio = new Studio(name, country, foundationDate, history);
            return Result<Studio>.AsSuccess(studio);
        }

        // Propriedades principais
        public Country Country { get; private set; }
        public DateTime FoundationDate { get; private set; }
        public string? History { get; private set; }

        // Propriedades de controle (padrão)
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Propriedades calculadas (exemplo)
        public int YearsInOperation => CalculateYearsInOperation(FoundationDate);

        #region Métodos de Negócio - Informações Básiscas
        public Result<bool> UpdateBasicInfo(string name, string? history = null)
        {
            var validationResult = Validate.NotNullOrEmpty(name, nameof(name))
                .Combine(Validate.MaxLength(name, MAX_NAME_LENGTH, nameof(name)));

            if (validationResult.IsFailure)
                return Result<bool>.AsFailure(validationResult.Failure!);

            if (!string.IsNullOrWhiteSpace(history))
            {
                var historyValidation = Validate.MaxLength(history, MAX_HISTORY_LENGTH, nameof(history));

                if (historyValidation.IsFailure)
                    return Result<bool>.AsFailure(historyValidation.Failure!);
            }

            Name = name.Trim();
            History = history?.Trim();
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

        public Result<bool> UpdateFoundationDate(DateTime foundationDate) 
        {
            var validationResult = Validate.IsPastDate(foundationDate, nameof(foundationDate), allowToday: true);

            if (validationResult.IsFailure)
                return Result<bool>.AsFailure(validationResult.Failure!);

            FoundationDate = foundationDate.Date;
            UpdatedAt = DateTime.UtcNow;

            return Result<bool>.AsSuccess(true);
        }
        #endregion

        #region Métodos de Negócio - Status
        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
        #endregion

        #region Métodos de Negócio - Regras Calculadas
        private static int CalculateYearsInOperation(DateTime foundationDate)
        {
            var today = DateTime.Today;
            var years = today.Year - foundationDate.Year;

            if (foundationDate.Date > today.AddYears(-years)) 
                years--;

            return years;
        }
        public override string ToString() => $"{Name} (Fundado em {FoundationDate.Year}) - {Country.Name}";
        #endregion
    }
}
