using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TreasuryFlow.Application.Auth.Services.Inputs;
using TreasuryFlow.Application.Auth.Services.Interfaces;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Application.Shared.Helpers;
using TreasuryFlow.Domain.Shared.Enums;
using TreasuryFlow.Domain.User.Entities;

namespace TreasuryFlow.Application.Auth.Services;

public class AuthService(ITreasuryFlowDbContext context, IConfiguration configuration) : IAuthService
{

    public async Task Register(RegisterInput input)
    {
        var user = new UserEntity
        {
            Name = input.Name,
            Email = input.Email,
            PasswordHash = PasswordHasherHelper.Hash(input.Password)
        };

        var hasAnyUser = await context.Users.AnyAsync();

        user.Role = !hasAnyUser
            ? EPermissionRole.Admin
            : EPermissionRole.User;

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task<string> Login(LoginInput input)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(x => x.Email == input.Email);

        if (user == null || !PasswordHasherHelper.Verify(input.Password, user.PasswordHash))
            throw new UnauthorizedAccessException();

        return GenerateToken(user);
    }

    private string GenerateToken(UserEntity user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

