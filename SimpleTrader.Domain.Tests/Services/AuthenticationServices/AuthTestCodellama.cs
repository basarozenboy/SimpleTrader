using NUnit.Framework.Internal;
using NUnit.Framework;
using SimpleTrader.Domain.Exceptions;
using SimpleTrader.Domain.Models;
using SimpleTrader.Domain.Services.AuthenticationServices;
using SimpleTrader.Domain.Services;
using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNet.Identity;
using Xunit;

namespace SimpleTrader.Domain.Tests.Services.AuthenticationServices
{
    internal class AuthTestCodellama
    {
        private readonly IAccountService _accountService;
        private readonly IPasswordHasher _passwordHasher;
        private AuthenticationService _authenticationService;
        // 1. Test case: Login with valid username and password
        [TestMethod]
public async Task Login_WithValidUsernameAndPassword_ReturnsAccount()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";
            Account expectedAccount = new Account()
            {
                AccountHolder = new User()
                {
                    Username = username,
                    PasswordHash = _passwordHasher.HashPassword(password)
                }
            };
            _accountService.GetByUsername(username).Returns(expectedAccount);

            // Act
            Account actualAccount = await _authenticationService.Login(username, password);

            // Assert
            Assert.AreEqual(expectedAccount, actualAccount);
        }
//2. Test case: Login with invalid username
        [TestMethod]
public async Task Login_WithInvalidUsername_ThrowsUserNotFoundException()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";
            _accountService.GetByUsername(username).Returns((Account)null);

            // Act
            Func<Task> action = async () => await _authenticationService.Login(username, password);

            // Assert
            await Assert.ThrowsExceptionAsync<UserNotFoundException>(action);
        }
//3. Test case: Login with invalid password
[TestMethod]
public async Task Login_WithInvalidPassword_ThrowsInvalidPasswordException()
        {
            // Arrange
            string username = "testuser";
            string password = "testpassword";
            Account storedAccount = new Account()
            {
                AccountHolder = new User()
                {
                    Username = username,
                    PasswordHash = _passwordHasher.HashPassword("invalidpassword")
                }
            };
            _accountService.GetByUsername(username).Returns(storedAccount);

            // Act
            Func<Task> action = async () => await _authenticationService.Login(username, password);

            // Assert
            await Microsoft.VisualStudio.TestTools.UnitTesting.Assert.ThrowsExceptionAsync<InvalidPasswordException>(action);
        }
//4. Test case: Register with valid email, username, password, and confirm password
[TestMethod]
public async Task Register_WithValidEmailUsernamePasswordAndConfirmPassword_ReturnsSuccess()
        {
            // Arrange
            string email = "test@example.com";
            string username = "testuser";
            string password = "testpassword";
            string confirmPassword = password;
            _accountService.GetByEmail(email).Returns((Account)null);
            _accountService.GetByUsername(username).Returns((Account)null);

            // Act
            RegistrationResult result = await _authenticationService.Register(email, username, password, confirmPassword);

            // Assert
            Assert.AreEqual(RegistrationResult.Success, result);
        }
//5. Test case: Register with invalid email
[TestMethod]
public async Task Register_WithInvalidEmail_ReturnsEmailAlreadyExists()
        {
            // Arrange
            string email = "test@example.com";
            string username = "testuser";
            string password = "testpassword";
            string confirmPassword = password;
            Account emailAccount = new Account()
            {
                AccountHolder = new User()
                {
                    Email = email
                }
            };
            _accountService.GetByEmail(email).Returns(emailAccount);

            // Act
            RegistrationResult result = await _authenticationService.Register(email, username, password, confirmPassword);

            // Assert
            Assert.AreEqual(RegistrationResult.EmailAlreadyExists, result);
        }
//6. Test case: Register with invalid username
[TestMethod]
public async Task Register_WithInvalidUsername_ReturnsUsernameAlreadyExists()
        {
            // Arrange
            string email = "test@example.com";
            string username = "testuser";
            string password = "testpassword";
            string confirmPassword = password;
            Account usernameAccount = new Account()
            {
                AccountHolder = new User()
                {
                    Username = username
                }
            };
            _accountService.GetByUsername(username).Returns(usernameAccount);

            // Act
            RegistrationResult result = await _authenticationService.Register(email, username, password, confirmPassword);

            // Assert
            Assert.AreEqual(RegistrationResult.UsernameAlreadyExists, result);
        }
//7. Test case: Register with non-matching passwords
[TestMethod]
public async Task Register_WithNonMatchingPasswords_ReturnsPasswordsDoNotMatch()
        {
            // Arrange
            string email = "test@example.com";
            string username = "testuser";
            string password = "testpassword";
            string confirmPassword = "invalidpassword";

            // Act
            RegistrationResult result = await _authenticationService.Register(email, username, password, confirmPassword);

            // Assert
            Assert.AreEqual(RegistrationResult.PasswordsDoNotMatch, result);
        }
//These unit tests cover various scenarios for the `Login` and `Register` methods of the `AuthenticationService` class. They verify that the methods behave as expected when provided with valid or invalid input.
    }
}
