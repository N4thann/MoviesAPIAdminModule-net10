using Domain.SmartEnums;
using Domain.ValueObjects;
using FluentAssertions;

namespace Domain.Tests.ValueObjectTests
{
    public class CreateAwardTests
    {

        [Fact]
        public void Create_WithValidData_ShouldReturnSuccess()
        {
            // Arrange
            var category = AwardCategory.BestPicture;
            var institution = Institution.AcademyAwards;
            var year = 2020;

            // Act
            var awardResult = Award.Create(category, institution, year);

            // Assert
            awardResult.IsSuccess.Should().BeTrue();
            awardResult.Success.Should().NotBeNull();
            awardResult.Success.Category.Should().Be(category);
            awardResult.Success.Institution.Should().Be(institution);
            awardResult.Success.Year.Should().Be(year);
        }

        [Fact]
        public void Create_WithNullCategory_ShouldReturnFailure()
        {
            // Arrange & Act
            var awardResult = Award.Create(null, Institution.AcademyAwards, 2020);

            // Assert
            awardResult.IsFailure.Should().BeTrue();
            awardResult.Failure.Code.Should().Be(400);
            awardResult.Failure.Message.Should().Contain("category");
        }

        [Fact]
        public void Create_WithNullInstitution_ShouldReturnFailure()
        {
            // Arrange & Act
            var awardResult = Award.Create(AwardCategory.BestPicture, null, 2020);

            // Assert
            awardResult.IsFailure.Should().BeTrue();
            awardResult.Failure.Code.Should().Be(400);
            awardResult.Failure.Message.Should().Contain("institution");
        }

        [Fact] 
        public void Create_WithFutureYearOutOfRange_ShouldReturnFailure()
        {
            var maxAllowedYear = DateTime.UtcNow.Year + 1;
            var invalidFutureYear = maxAllowedYear + 1;

            // Act
            var awardResult = Award.Create(AwardCategory.BestPicture, Institution.AcademyAwards, invalidFutureYear);

            // Assert
            awardResult.IsFailure.Should().BeTrue();
            awardResult.Failure.Code.Should().Be(400);
            awardResult.Failure.Message.Should().Contain("year");
        }

        [Fact]
        public void Create_WithPastYearOutOfRange_ShouldReturnFailure()
        {
            // Arrange
            var invalidPastYear = 1879;

            // Act
            var awardResult = Award.Create(AwardCategory.BestPicture, Institution.AcademyAwards, invalidPastYear);

            // Assert
            awardResult.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void TwoAwardsWithSameValues_ShouldBeEqual()
        {
            // Arrange
            var award1 = Award.Create(AwardCategory.BestDirector, Institution.BAFTA, 2021).Success!;
            var award2 = Award.Create(AwardCategory.BestDirector, Institution.BAFTA, 2021).Success!;

            // Act & Assert
            award1.Should().Be(award2);
            (award1 == award2).Should().BeTrue();
            (award1 != award2).Should().BeFalse();

            award1.Id.Should().NotBe(award2.Id);
        }

        [Fact]
        public void TwoAwardsWithDifferentValues_ShouldNotBeEqual()
        {
            // Arrange
            var award1 = Award.Create(AwardCategory.BestActor, Institution.GoldenGlobes, 2022).Success!;

            var awardWithDifferentCategory = Award.Create(AwardCategory.BestActress, Institution.GoldenGlobes, 2022).Success!;
            var awardWithDifferentInstitution = Award.Create(AwardCategory.BestActor, Institution.SAG, 2022).Success!;
            var awardWithDifferentYear = Award.Create(AwardCategory.BestActor, Institution.GoldenGlobes, 2023).Success!;

            // Act & Assert
            award1.Should().NotBe(awardWithDifferentCategory);
            award1.Should().NotBe(awardWithDifferentInstitution);
            award1.Should().NotBe(awardWithDifferentYear);
            (award1 == awardWithDifferentYear).Should().BeFalse();
        }

        [Fact]
        public void ToString_ShouldReturnCorrectlyFormattedString()
        {
            // Arrange
            var award = Award.Create(AwardCategory.PalmeDor, Institution.CannesFestival, 2019).Success!;
            var expectedString = "Palme d'Or (Cannes Film Festival, 2019)";

            // Act
            var resultString = award.ToString();

            // Assert
            resultString.Should().Be(expectedString);
        }
    }
}
