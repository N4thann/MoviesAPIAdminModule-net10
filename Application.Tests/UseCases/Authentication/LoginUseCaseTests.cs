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
        private readonly IOptions<JwtOptions> _subJwtOptions; // Mudou aqui

        private readonly Faker _faker;
        private readonly ApplicationUser _validUser;
        private readonly LoginCommand _loginCommand;
        private readonly JwtOptions _fakeJwtOptions;

        public LoginUseCaseTests()
        {
            _faker = new Faker("pt_BR");

            // 1. Setup do UserManager (Mock complexo do Identity)
            var subUserStore = Substitute.For<IUserStore<ApplicationUser>>();
            _subUserManager = Substitute.For<UserManager<ApplicationUser>>(
                subUserStore, null, null, null, null, null, null, null, null);

            _subTokenService = Substitute.For<ITokenService>();

            // 2. Setup do Options Pattern
            _fakeJwtOptions = new JwtOptions
            {
                RefreshTokenValidityInMinutes = 60,
                // Adicione outras propriedades do seu JwtOptions se necessário
            };
            _subJwtOptions = Substitute.For<IOptions<JwtOptions>>();
            _subJwtOptions.Value.Returns(_fakeJwtOptions);

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

            // 3. SUT com o novo construtor
            _sut = new LoginUseCase(
                _subUserManager,
                _subTokenService,
                _subJwtOptions
            );
        }

        [Fact]
        public async Task Handle_WhenCredentialsAreValid_ShouldReturnSuccessTokenResponse()
        {
            // Arrange
            var userRoles = new List<string> { "Admin" };
            var fakeRefreshToken = _faker.Random.AlphaNumeric(32);
            var fakeExpiration = DateTime.UtcNow.AddHours(1);
            var fakeJwtToken = new JwtSecurityToken(expires: fakeExpiration);

            _subUserManager.FindByNameAsync(_loginCommand.UserName).Returns(_validUser);
            _subUserManager.CheckPasswordAsync(_validUser, _loginCommand.Password).Returns(true);
            _subUserManager.GetRolesAsync(_validUser).Returns(userRoles);

            _subTokenService.GenerateAccessToken(Arg.Any<List<Claim>>(), _fakeJwtOptions).Returns(fakeJwtToken);
            _subTokenService.GenerateRefreshToken().Returns(fakeRefreshToken);
            _subUserManager.UpdateAsync(_validUser).Returns(IdentityResult.Success);

            // Act
            var result = await _sut.Handle(_loginCommand, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Success.RefreshToken.Should().Be(fakeRefreshToken);
            _validUser.RefreshToken.Should().Be(fakeRefreshToken);
        }

        [Fact]
        public async Task Handle_WhenUserHasNoRequiredRole_ShouldReturnForbiddenFailure()
        {
            // Arrange
            var userRoles = new List<string> { "User" }; // Role que não está na lista _adminRoles do UseCase
            _subUserManager.FindByNameAsync(_loginCommand.UserName).Returns(_validUser);
            _subUserManager.CheckPasswordAsync(_validUser, _loginCommand.Password).Returns(true);
            _subUserManager.GetRolesAsync(_validUser).Returns(userRoles);

            // Act
            var result = await _sut.Handle(_loginCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Failure.Type.Should().Be(FailureType.Forbidden);
        }

        [Fact]
        public async Task Handle_WhenUpdateAsyncFails_ShouldReturnInfrastructureFailure()
        {
            // Arrange
            var userRoles = new List<string> { "Admin" };
            var fakeJwtToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddHours(1));
            _subUserManager.FindByNameAsync(_loginCommand.UserName).Returns(_validUser);
            _subUserManager.CheckPasswordAsync(_validUser, _loginCommand.Password).Returns(true);
            _subUserManager.GetRolesAsync(_validUser).Returns(userRoles);
            _subTokenService.GenerateAccessToken(Arg.Any<List<Claim>>(), _fakeJwtOptions).Returns(fakeJwtToken);
            _subTokenService.GenerateRefreshToken().Returns("token");

            var updateError = new IdentityError { Description = "DB Error" };
            _subUserManager.UpdateAsync(_validUser).Returns(IdentityResult.Failed(updateError));

            // Act
            var result = await _sut.Handle(_loginCommand, CancellationToken.None);

            // Assert
            result.IsFailure.Should().BeTrue();
            result.Failure.Message.Should().Contain("Failed to save refresh token");
        }
    }
}