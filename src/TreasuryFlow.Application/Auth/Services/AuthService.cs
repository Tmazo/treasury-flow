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
using TreasuryFlow.Domain.Owner.Entities;

namespace TreasuryFlow.Application.Auth.Services;

public class AuthService(ITreasuryFlowDbContext context, IConfiguration configuration) : IAuthService
{

    public async Task Register(RegisterInput input)
    {
        var owner = new OwnerEntity
        {
            Name = input.Name,
            Email = input.Email,
            PasswordHash = PasswordHasherHelper.Hash(input.Password)
        };

        await context.Owners.AddAsync(owner);
        await context.SaveChangesAsync();
    }

    public async Task<string> Login(LoginInput input)
    {
        var owner = await context.Owners
            .FirstOrDefaultAsync(x => x.Email == input.Email);

        if (owner == null || !PasswordHasherHelper.Verify(input.Password, owner.PasswordHash))
            throw new UnauthorizedAccessException();

        return GenerateToken(owner);
    }

    private string GenerateToken(OwnerEntity owner)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, owner.Id.ToString()),
            new Claim(ClaimTypes.Name, owner.Email)
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

