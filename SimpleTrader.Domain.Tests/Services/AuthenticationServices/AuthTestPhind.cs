using Microsoft.AspNet.Identity;
using Moq;
using SimpleTrader.Domain.Exceptions;
using SimpleTrader.Domain.Models;
using SimpleTrader.Domain.Services.AuthenticationServices;
using SimpleTrader.Domain.Services;
using System.Threading.Tasks;
using System.Threading;
using System;
using Xunit;
using Xunit;
using Moq;
using SimpleTrader.Domain.Models;
using SimpleTrader.Domain.Exceptions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace SimpleTrader.Domain.Tests.Services.AuthenticationServices
{
    public class AuthenticationServiceTests2
    {
        [Fact]
        public async Task Login_Success()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";
            string hashedPassword = "hashedpassword";

            var accountServiceMock = new Mock<IAccountService>();
            accountServiceMock.Setup(x => x.GetByUsername(username))
                .ReturnsAsync(new Account
                {
                    AccountHolder = new User
                    {
                        Username = username,
                        PasswordHash = hashedPassword
                    }
                });

            var passwordHasher = new Mock<IPasswordHasher>();
            passwordHasher.Setup(x => x.VerifyHashedPassword(hashedPassword, password))
                .Returns(PasswordVerificationResult.Success);

            var authenticationService = new AuthenticationService(accountServiceMock.Object, passwordHasher.Object);

            // Act
            var result = await authenticationService.Login(username, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(username, result.AccountHolder.Username);
            Assert.Equal(hashedPassword, result.AccountHolder.PasswordHash);
        }

        [Fact]
        public async Task Login_UserNotFound()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";

            var accountServiceMock = new Mock<IAccountService>();
            accountServiceMock.Setup(x => x.GetByUsername(username))
                .ReturnsAsync((Account)null);

            var passwordHasher = new Mock<IPasswordHasher>();

            var authenticationService = new AuthenticationService(accountServiceMock.Object, passwordHasher.Object);

            // Act & Assert
            await Assert.ThrowsAsync<UserNotFoundException>(() => authenticationService.Login(username, password));
        }

        [Fact]
        public async Task Login_InvalidPassword()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";
            string hashedPassword = "hashedpassword";

            var accountServiceMock = new Mock<IAccountService>();
            accountServiceMock.Setup(x => x.GetByUsername(username))
                .ReturnsAsync(new Account
                {
                    AccountHolder = new User
                    {
                        Username = username,
                        PasswordHash = hashedPassword
                    }
                });

            var passwordHasher = new Mock<IPasswordHasher>();
            passwordHasher.Setup(x => x.VerifyHashedPassword(hashedPassword, password))
                .Returns(PasswordVerificationResult.Failed);

            var authenticationService = new AuthenticationService(accountServiceMock.Object, passwordHasher.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidPasswordException>(() => authenticationService.Login(username, password));
        }

        // Add more tests for Register method as needed
    }
}