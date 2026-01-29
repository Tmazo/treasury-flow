using FluentAssertions;
using Microsoft.Extensions.Configuration;
using TreasuryFlow.Application.Auth.Services;
using TreasuryFlow.Application.Auth.Services.Inputs;
using TreasuryFlow.Application.Shared.Helpers;
using TreasuryFlow.Application.Tests.Fixtures;
using TreasuryFlow.Domain.Shared.Enums;
using TreasuryFlow.Domain.User.Entities;
using TreasuryFlow.Infrastructure.Shared.Data;

namespace TreasuryFlow.Application.Tests.AuthTests.ServicesTests.AuthServiceTests;

public class LoginTests : IClassFixture<TreasuryFlowDbContextFixture>
{
    readonly TreasuryFlowDbContext _context;
    readonly TreasuryFlowDbContextFixture _fixture;
    readonly AuthService _service;
    readonly IConfiguration _configuration;

    public LoginTests(TreasuryFlowDbContextFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.DbContext;

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "super-secret-key-super-secret-key",
                ["Jwt:Issuer"] = "treasuryflow",
                ["Jwt:Audience"] = "treasuryflow"
            })
            .Build();

        _service = new AuthService(_context, _configuration);
    }

    [Fact]
    public async Task Login_WhenCredentialsAreValid_ShouldReturnToken()
    {
        // Arrange
        var password = "123456";

        var user = new UserEntity
        {
            Name = "Test User",
            Email = "user@treasuryflow.com",
            PasswordHash = PasswordHasherHelper.Hash(password),
            Role = EPermissionRole.User
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var input = new LoginInput(
            user.Name,
            user.Email,
            password
        );

        // Act
        var token = await _service.Login(input);

        // Assert
        token.Should().NotBeNullOrWhiteSpace();

        await _fixture.ResetDatabaseAsync();
    }

    [Fact]
    public async Task Login_WhenUserDoesNotExist_ShouldThrowUnauthorized()
    {
        // Arrange
        var input = new LoginInput(
            "Unknown",
            "notfound@treasuryflow.com",
            "123456"
        );

        // Act
        Func<Task> act = () => _service.Login(input);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>();

        await _fixture.ResetDatabaseAsync();
    }

    [Fact]
    public async Task Login_WhenPasswordIsInvalid_ShouldThrowUnauthorized()
    {
        // Arrange
        var user = new UserEntity
        {
            Name = "Test User",
            Email = "user@treasuryflow.com",
            PasswordHash = PasswordHasherHelper.Hash("correct-password"),
            Role = EPermissionRole.User
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var input = new LoginInput(
            user.Name,
            user.Email,
            "wrong-password"
        );

        // Act
        Func<Task> act = () => _service.Login(input);

        // Assert
        await act.Should()
            .ThrowAsync<UnauthorizedAccessException>();

        await _fixture.ResetDatabaseAsync();
    }
}
