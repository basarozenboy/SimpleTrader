using System.Threading.Tasks;
using Xunit;
using Moq;
using SimpleTrader.Domain.Models;
using SimpleTrader.Domain.Exceptions;
using Microsoft.AspNet.Identity;

namespace SimpleTrader.Domain.Services.AuthenticationServices.Tests
{
    public class AuthenticationServiceTests
    {
        private readonly Mock<IAccountService> _accountServiceMock;
        private readonly Mock<IPasswordHasher> _passwordHasherMock;
        private readonly AuthenticationService _authenticationService;

        public AuthenticationServiceTests()
        {
            _accountServiceMock = new Mock<IAccountService>();
            _passwordHasherMock = new Mock<IPasswordHasher>();
            _authenticationService = new AuthenticationService(_accountServiceMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public async Task Login_ThrowsUserNotFoundException_WhenUserDoesNotExist()
        {
            // Arrange
            _accountServiceMock.Setup(x => x.GetByUsername(It.IsAny<string>())).ReturnsAsync((Account)null);

            // Act and Assert
            await Assert.ThrowsAsync<UserNotFoundException>(async () => await _authenticationService.Login("username", "password"));
        }

        [Fact]
        public async Task Login_ThrowsInvalidPasswordException_WhenPasswordIsIncorrect()
        {
            // Arrange
            var account = new Account();
            _accountServiceMock.Setup(x => x.GetByUsername(It.IsAny<string>())).ReturnsAsync(account);
            _passwordHasherMock.Setup(x => x.VerifyHashedPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(PasswordVerificationResult.Failed);

            // Act and Assert
            await Assert.ThrowsAsync<InvalidPasswordException>(async () => await _authenticationService.Login("username", "password"));
        }

        [Fact]
        public async Task Register_ReturnsPasswordsDoNotMatch_WhenPasswordsDoNotMatch()
        {
            // Arrange
            var result = await _authenticationService.Register("email", "username", "password", "differentPassword");

            // Assert
            Assert.Equal(RegistrationResult.PasswordsDoNotMatch, result);
        }

        // Add more test cases as needed...
    }
}