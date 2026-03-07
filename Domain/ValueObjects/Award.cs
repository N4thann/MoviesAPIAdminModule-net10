using Domain.SeedWork;
using Domain.SeedWork.Core;
using Domain.SeedWork.Validation;
using Domain.SmartEnums;

namespace Domain.ValueObjects
{
    public class Award : ValueObject
    {
        private const int MIN_YEAR = 1880;
        private static readonly int MAX_YEAR = DateTime.UtcNow.Year + 1;

        private Award() { }

        private Award(AwardCategory category, Institution institution, int year)
        {
            Id = Guid.NewGuid();
            Category = category;
            Institution = institution;
            Year = year;
        }

        public static Result<Award> Create(AwardCategory category, Institution institution, int year)
        {
            var validation1 = Validate.NotNull(category, nameof(category));

            var validation2 = Validate.NotNull(institution, nameof(institution));

            var validation3 = Validate.Range(year, MIN_YEAR, MAX_YEAR, nameof(year));

            if (validation1.IsFailure)
                return Result<Award>.AsFailure(validation1.Failure!);
            if (validation2.IsFailure)
                return Result<Award>.AsFailure(validation2.Failure!);
            if (validation3.IsFailure)
                return Result<Award>.AsFailure(validation3.Failure!);

            var award = new Award(category, institution, year);

            return Result<Award>.AsSuccess(award);
        }

        public Guid Id { get; private set; }
        public AwardCategory Category { get; private set; } = null!;
        public Institution Institution { get; private set; } = null!;
        public int Year { get; private set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Category;
            yield return Institution;
            yield return Year;
        }

        public override string ToString() => $"{Category.Name} ({Institution.Name}, {Year})";
    }
}