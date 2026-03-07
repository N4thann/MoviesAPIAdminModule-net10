using Application.Commands.Authentication;
using Application.Common; // Certifique-se que o JwtOptions está aqui
using Application.Interfaces;
using Application.UseCases.Authentication;
using Bogus;
using Domain.Enums;
using Domain.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Application.Tests.UseCases.Authentication
{
    public class LoginUseCaseTests
    {
        private readonly LoginUseCase _sut;
        private readonly UserManager<ApplicationUser> _subUserManager;
        private readonly ITokenService _subTokenService;
        private readonly IOptions<JwtOptions> _subJwtOptions;

        private readonly Faker _faker;
        private readonly ApplicationUser _validUser;
        private readonly LoginCommand _loginCommand;
        private readonly JwtOptions _fakeJwtOptions;

        public LoginUseCaseTests()
        {
            _faker = new Faker("pt_BR");

            // Setup Mocks
            var subUserStore = Substitute.For<IUserStore<ApplicationUser>>();
            _subUserManager = Substitute.For<UserManager<ApplicationUser>>(
                subUserStore, null!, null!, null!, null!, null!, null!, null!, null!);

            _subTokenService = Substitute.For<ITokenService>();

            _fakeJwtOptions = new JwtOptions { RefreshTokenValidityInMinutes = 60 };
            _subJwtOptions = Substitute.For<IOptions<JwtOptions>>();
            _subJwtOptions.Value.Returns(_fakeJwtOptions);

            // Setup Data
            _loginCommand = new LoginCommand(
                UserName: _faker.Random.String2(8, "abcdefghijklmnopqrstuvwxyz"),
                Password: $"P@ss{_faker.Random.Number(99)}w{_faker.Lorem.Letter()}!"
            );

            _validUser = new ApplicationUser
            {
                UserName = _loginCommand.UserName,
                Email = _faker.Internet.Email(),
                RefreshToken = _faker.Random.AlphaNumeric(20),
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7)
            };

            _sut = new LoginUseCase(_subUserManager, _subTokenService, _subJwtOptions);
        }

        #region Helper Methods (Redução de Duplicação/Sonar)

        private void SetupUserFlow(bool passwordValid, List<string> roles)
        {
            _subUserManager.FindByNameAsync(_loginCommand.UserName).Returns(_validUser);
            _subUserManager.CheckPasswordAsync(_validUser, _loginCommand.Password).Returns(passwordValid);
            _subUserManager.GetRolesAsync(_validUser).Returns(roles);
        }

        private void SetupTokenService(string refreshToken, DateTime expiration)
        {
            var fakeJwtToken = new JwtSecurityToken(expires: expiration);
            _subTokenService.GenerateAccessToken(Arg.Any<List<Claim>>(), _fakeJwtOptions).Returns(fakeJwtToken);
            _subTokenService.GenerateRefreshToken().Returns(refreshToken);
        }

        #endregion

        [Fact]
        public async Task Handle_WhenCredentialsAreValid_ShouldReturnSuccessTokenResponse()
        {
            var fakeRefreshToken = _faker.Random.AlphaNumeric(32);
            var fakeExpiration = DateTime.UtcNow.AddHours(1);

            SetupUserFlow(passwordValid: true, roles: new List<string> { "Admin" });
            SetupTokenService(fakeRefreshToken, fakeExpiration);
            _subUserManager.UpdateAsync(_validUser).Returns(IdentityResult.Success);

            // Act
            var result = await _sut.Handle(_loginCommand, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Success!.RefreshToken.Should().Be(fakeRefreshToken);
            _validUser.RefreshToken.Should().Be(fakeRefreshToken);
        }

        [Fact]
        public async Task Handle_WhenUserHasNoRequiredRole_ShouldReturnForbiddenFailure()
        {
            // Arrange
            SetupUserFlow(passwordValid: true, roles: new List<string> { "User" });

            // Act
            var result = await _sut.Handle(_loginCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Failure!.Type.Should().Be(FailureType.Forbidden);
        }

        [Fact]
        public async Task Handle_WhenUpdateAsyncFails_ShouldReturnInfrastructureFailure()
        {
            // Arrange
            SetupUserFlow(passwordValid: true, roles: new List<string> { "Admin" });
            SetupTokenService("token", DateTime.UtcNow.AddHours(1));

            var updateError = new IdentityError { Description = "DB Error" };
            _subUserManager.UpdateAsync(_validUser).Returns(IdentityResult.Failed(updateError));

            // Act
            var result = await _sut.Handle(_loginCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Failure!.Message.Should().Contain("Failed to save refresh token");
        }
    }
}