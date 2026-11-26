using DailyGlowApi.Dtos;
using DailyGlowApi.Models;
using DailyGlowApi.Repositories;
using DailyGlowApi.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DailyGlowApi.Tests;

// Unit tests for AuthService. The user repository and the token maker are faked with Moq.
// The password hasher is the real one, so the hashing is really tested.
// Based on Microsoft Learn: Unit testing C# with xUnit
// https://learn.microsoft.com/dotnet/core/testing/unit-testing-csharp-with-xunit
// and the Moq Quickstart: https://github.com/devlooped/moq/wiki/Quickstart
public class AuthServiceTests
{
    private const string Password = "Glow12345";

    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<ITokenService> _tokens = new();
    private readonly PasswordHasher<User> _hasher = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_users.Object, _hasher, _tokens.Object);
    }

    private User MakeUser()
    {
        var user = new User { Id = 1, Email = "student@example.com" };
        user.PasswordHash = _hasher.HashPassword(user, Password);
        return user;
    }

    [Fact]
    public async Task RegisterAsync_ReturnsEmailTaken_WhenEmailIsAlreadyUsed()
    {
        // Arrange
        _users.Setup(u => u.EmailExistsAsync("student@example.com")).ReturnsAsync(true);
        var dto = new RegisterDto { Email = " Student@Example.com ", Password = Password };

        // Act
        var result = await _service.RegisterAsync(dto);

        // Assert
        Assert.Equal(RegisterResult.EmailTaken, result);
        _users.Verify(u => u.AddAsync(It.IsAny<User>()), Times.Never());
    }

    [Fact]
    public async Task RegisterAsync_SavesLowerCaseEmail_AndAHashedPassword()
    {
        User? saved = null;
        _users.Setup(u => u.EmailExistsAsync("student@example.com")).ReturnsAsync(false);
        _users.Setup(u => u.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User newUser) =>
            {
                saved = newUser;
                return newUser;
            });
        var dto = new RegisterDto { Email = "  Student@Example.com ", Password = Password };

        var result = await _service.RegisterAsync(dto);

        Assert.Equal(RegisterResult.Registered, result);
        Assert.NotNull(saved);
        Assert.Equal("student@example.com", saved!.Email);

        // The real password must never be saved, only the hash
        Assert.NotEqual(Password, saved.PasswordHash);
        var check = _hasher.VerifyHashedPassword(saved, saved.PasswordHash, Password);
        Assert.Equal(PasswordVerificationResult.Success, check);
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenEmailIsUnknown()
    {
        _users.Setup(u => u.GetByEmailAsync("nobody@example.com")).ReturnsAsync((User?)null);
        var dto = new LoginDto { Email = "nobody@example.com", Password = Password };

        var result = await _service.LoginAsync(dto);

        Assert.Null(result);
        _tokens.Verify(t => t.CreateToken(It.IsAny<User>()), Times.Never());
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_WhenPasswordIsWrong()
    {
        var user = MakeUser();
        _users.Setup(u => u.GetByEmailAsync("student@example.com")).ReturnsAsync(user);
        var dto = new LoginDto { Email = "student@example.com", Password = "WrongPassword1" };

        var result = await _service.LoginAsync(dto);

        Assert.Null(result);
        _tokens.Verify(t => t.CreateToken(It.IsAny<User>()), Times.Never());
    }

    [Fact]
    public async Task LoginAsync_ReturnsTheToken_WhenEmailAndPasswordAreCorrect()
    {
        var user = MakeUser();
        var expires = new DateTime(2030, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        _users.Setup(u => u.GetByEmailAsync("student@example.com")).ReturnsAsync(user);
        _tokens.Setup(t => t.CreateToken(user)).Returns(new TokenResult("fake-token", expires));
        var dto = new LoginDto { Email = "student@example.com", Password = Password };

        var result = await _service.LoginAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("fake-token", result!.Token);
        Assert.Equal(expires, result.ExpiresAtUtc);
    }

    [Fact]
    public async Task LoginAsync_IgnoresCapitalLettersAndSpacesInTheEmail()
    {
        var user = MakeUser();
        _users.Setup(u => u.GetByEmailAsync("student@example.com")).ReturnsAsync(user);
        _tokens.Setup(t => t.CreateToken(user)).Returns(new TokenResult("fake-token", DateTime.UtcNow.AddHours(1)));
        var dto = new LoginDto { Email = "  STUDENT@Example.com ", Password = Password };

        var result = await _service.LoginAsync(dto);

        Assert.NotNull(result);
    }
}