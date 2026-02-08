using Domain.Entities;
using Domain.SeedWork;
using Domain.SeedWork.Core;

namespace Domain.ValueObjects
{
    public class Rating : ValueObject
    {
        public Rating() { }

        private Rating(decimal totalSum, int votesCount, decimal maxValue)
        {
            TotalSum = totalSum;
            VotesCount = votesCount;
            MaxValue = maxValue;
        }

        public static Result<Rating> Create(decimal totalSum, int votesCount, decimal maxValue = 10)
        {
            if (maxValue <= 0)
                return Result<Rating>.AsFailure(Failure.Validation("Max value must be greater than 0"));

            if (totalSum < 0)
                return Result<Rating>.AsFailure(Failure.Validation("Total sum cannot be negative"));

            if (votesCount > 0 && totalSum > (votesCount * maxValue))
                return Result<Rating>.AsFailure(Failure.Validation("Total sum exceeds maximum possible value for given votes count"));

            var rating = new Rating(totalSum,votesCount,maxValue);

            return Result<Rating>.AsSuccess(rating);
        }

        public decimal TotalSum { get; }
        public int VotesCount { get; }
        public decimal MaxValue { get; }

        // Propriedades calculadas
        public decimal AverageValue => HasVotes ? TotalSum / VotesCount : 0;
        public bool HasVotes => VotesCount > 0;

        public decimal NormalizedScore => HasVotes ? AverageValue / MaxValue : 0;

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return TotalSum;
            yield return VotesCount;
        }

        public override string ToString()
        {
            if (!HasVotes)
                return "No ratings";

            return $"{AverageValue:F1}/{MaxValue} ({VotesCount} votes)";
        }

        public static Rating CreateEmpty(decimal maxValue = 10) => new Rating(0, 0, maxValue);
    }
}
