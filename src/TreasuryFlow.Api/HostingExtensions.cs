using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TreasuryFlow.Api.Auth.Requirements;
using TreasuryFlow.Api.Auth.Requirements.Handlers;
using TreasuryFlow.Application;
using TreasuryFlow.Infrastructure;
using TreasuryFlow.Infrastructure.Shared.Communications;

namespace TreasuryFlow.Api;

public static class HostingExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddServices();
        builder.Services.AddDatabase(builder.Configuration);

        builder.Services.AddMassTransitDefaults(configure =>
        {
            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.AddRabbitMqHost(context);
            });
        });

        builder.Services.AddAuthentication("Bearer")
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                    )
                };
            });

        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireUserId", policy =>
                policy.Requirements.Add(new RequireUserIdRequirement()));
        });

        builder.Services.AddSingleton<IAuthorizationHandler, RequireUserIdHandler>();

        return builder;
    }
}
