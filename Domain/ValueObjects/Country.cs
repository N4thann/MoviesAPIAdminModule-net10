using Domain.Entities;
using Domain.SeedWork;
using Domain.SeedWork.Core;
using Domain.SeedWork.Validation;


namespace Domain.ValueObjects
{
    public class Country : ValueObject
    {
        private Country() { }

        private Country(string name, string code) 
        {
            Name = name;
            Code = code;
        }

        public static Result<Country> Create(string name, string code)
        {
            name = name.Trim();
            code = code.Trim().ToUpperInvariant();

            var validationResult = Validate.NotNullOrEmpty(name, nameof(name))
                .Combine(
                Validate.NotNullOrEmpty(code, nameof(code)),
                Validate.MaxLength(name, 100, nameof(name)),
                Validate.MinLength(name, 2, nameof(name)),
                Validate.MinLength(code, 2, nameof(code)),
                Validate.MaxLength(code, 3, nameof(code)));

            if (validationResult.IsFailure)
                return Result<Country>.AsFailure(validationResult.Failure!);

            var country = new Country(name, code);

            return Result<Country>.AsSuccess(country);
        }

        public string Name { get; private set; } = null!;
        public string Code { get; private set; } = null!;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }

        public override string ToString() => $"Name: {Name} / Code: {Code}";

        //Factory Methods
        public static Country Brazil => new("Brazil", "BR");
        public static Country UnitedStates => new("United States", "USA");
        public static Country UnitedKingdom => new("United Kingdom", "GB");
        public static Country France => new("France", "FR");
        public static Country Germany => new("Germany", "DE");
        public static Country Japan => new("Japan", "JP");
        public static Country China => new("China", "CN");
        public static Country India => new("India", "IN");
        public static Country Canada => new("Canada", "CA");
        public static Country Australia => new("Australia", "AU");
    }
}
