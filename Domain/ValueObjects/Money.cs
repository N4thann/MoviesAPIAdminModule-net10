using Domain.Entities;
using Domain.SeedWork;
using Domain.SeedWork.Core;
using Domain.SeedWork.Validation;

namespace Domain.ValueObjects
{
    public class Money : ValueObject
    {
        // EF Core utiliza o construtor vazio via reflexão para depois preencher as propriedades
        private Money() { }

        private Money(decimal amount, string currency) 
        {
            Amount = amount;
            Currency = currency.ToUpperInvariant();
        }

        public static Result<Money> Create(decimal amount, string currency = "USD")
        {
            var validationResult = Validate.GreaterThan((int)amount, -1, nameof(amount))
                .Combine(
                    Validate.NotNullOrEmpty(currency, nameof(currency)),
                    Validate.MaxLength(currency, 3, nameof(currency)));

            if (validationResult.IsFailure)
                return Result<Money>.AsFailure(validationResult.Failure!);

            var money = new Money(amount, currency);

            return Result<Money>.AsSuccess(money);
        }

        public decimal Amount { get; private set; }

        public string Currency { get; private set; } = null!;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        public override string ToString()
        {
            return Currency switch
            {
                "USD" => $"${Amount:N2} {Currency}",
                "BRL" => $"R${Amount:N2} {Currency}",
                "EUR" => $"€{Amount:N2} {Currency}",
                "GBP" => $"£{Amount:N2} {Currency}",
                _ => $"{Amount:N2} {Currency}"
            };
        }

        //Factory Methods
        public static Money Zero(string currency = "USD") => new(0, currency);
        public static Money Dollars(decimal amount) => new(amount, "USD");
        public static Money Reais(decimal amount) => new(amount, "BRL");
        public static Money Euros(decimal amount) => new(amount, "EUR");
    }
}
