using FluentAssertions;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TreasuryFlow.Application.Auth.Services;
using TreasuryFlow.Application.Auth.Services.Inputs;
using TreasuryFlow.Application.Shared.Helpers;
using TreasuryFlow.Application.Tests.Fixtures;
using TreasuryFlow.Domain.Shared.Enums;
using TreasuryFlow.Domain.User.Entities;
using TreasuryFlow.Infrastructure.Shared.Data;

namespace TreasuryFlow.Application.Tests.AuthTests.ServicesTests.AuthServiceTests;

public class GenerateTokenTests : IClassFixture<TreasuryFlowDbContextFixture>
{
    readonly TreasuryFlowDbContext _context;
    readonly TreasuryFlowDbContextFixture _fixture;
    readonly AuthService _service;
    readonly IConfiguration _configuration;

    public GenerateTokenTests(TreasuryFlowDbContextFixture fixture)
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
    public async Task Login_ShouldGenerateJwtWithExpectedClaims()
    {
        // Arrange
        var password = "123456";

        var user = new UserEntity
        {
            Name = "Token User",
            Email = "token@treasuryflow.com",
            PasswordHash = PasswordHasherHelper.Hash(password),
            Role = EPermissionRole.Admin
        };

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        var input = new LoginInput(
            user.Name,
            user.Email,
            password
        );

        // Act
        var tokenString = await _service.Login(input);

        // Assert
        tokenString.Should().NotBeNullOrWhiteSpace();

        var tokenHandler = new JwtSecurityTokenHandler();
        var jwt = tokenHandler.ReadJwtToken(tokenString);

        jwt.Issuer.Should().Be(_configuration["Jwt:Issuer"]);
        jwt.Audiences.Should().Contain(_configuration["Jwt:Audience"]);

        jwt.Claims.Should().Contain(c =>
            c.Type == ClaimTypes.NameIdentifier &&
            c.Value == user.Id.ToString());

        jwt.Claims.Should().Contain(c =>
            c.Type == ClaimTypes.Name &&
            c.Value == user.Email);

        jwt.Claims.Should().Contain(c =>
            c.Type == ClaimTypes.Role &&
            c.Value == user.Role.ToString());

        await _fixture.ResetDatabaseAsync();
    }
}
