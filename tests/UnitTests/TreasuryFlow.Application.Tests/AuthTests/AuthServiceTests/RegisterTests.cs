using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using TreasuryFlow.Application.Auth.Services;
using TreasuryFlow.Application.Auth.Services.Inputs;
using TreasuryFlow.Application.Tests.Fixtures;
using TreasuryFlow.Domain.Shared.Enums;
using TreasuryFlow.Domain.User.Entities;
using TreasuryFlow.Infrastructure.Shared.Data;

namespace TreasuryFlow.Application.Tests.AuthTests.ServicesTests.AuthServiceTests;

public class RegisterTests : IClassFixture<TreasuryFlowDbContextFixture>
{
    readonly TreasuryFlowDbContext _context;
    readonly TreasuryFlowDbContextFixture _fixture;
    readonly AuthService _service;

    public RegisterTests(TreasuryFlowDbContextFixture fixture)
    {
        _fixture = fixture;
        _context = _fixture.DbContext;

        var configuration = Substitute.For<IConfiguration>();

        _service = new AuthService(_context, configuration);
    }

    [Fact]
    public async Task Register_WhenNoUserExists_ShouldCreateAdminUser()
    {
        // Arrange
        var input = new RegisterInput("Admin User", "admin@treasuryflow.com", "123456");

        // Act
        await _service.Register(input);

        // Assert
        var user = await _context.Users.SingleAsync();

        user.Name.Should().Be(input.Name);
        user.Email.Should().Be(input.Email);
        user.Role.Should().Be(EPermissionRole.Admin);

        user.PasswordHash.Should().NotBe(input.Password);
        user.PasswordHash.Should().NotBeNullOrWhiteSpace();

        await _fixture.ResetDatabaseAsync();
    }

    [Fact]
    public async Task Register_WhenUserAlreadyExists_ShouldCreateUserWithUserRole()
    {
        // Arrange
        var existingUser = new UserEntity
        {
            Name = "Existing User",
            Email = "existing@treasuryflow.com",
            PasswordHash = "hashed-password",
            Role = EPermissionRole.Admin
        };

        await _context.Users.AddAsync(existingUser);
        await _context.SaveChangesAsync();

        var input = new RegisterInput("New User", "user@treasuryflow.com", "654321");

        // Act
        await _service.Register(input);

        // Assert
        var users = await _context.Users.ToListAsync();

        users.Should().HaveCount(2);

        var newUser = users.Single(u => u.Email == input.Email);

        newUser.Role.Should().Be(EPermissionRole.User);
        newUser.PasswordHash.Should().NotBe(input.Password);
        newUser.PasswordHash.Should().NotBeNullOrWhiteSpace();

        await _fixture.ResetDatabaseAsync();
    }
}
