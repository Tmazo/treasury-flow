using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TreasuryFlow.Api.Auth.Requirements;
using TreasuryFlow.Api.Auth.Requirements.Handlers;
using TreasuryFlow.Api.UserBalances.Validators;
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
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Informe o token JWT no formato: Bearer {seu_token}"
            });

            options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });


        builder.Services.AddServices();
        builder.Services.AddDatabase(builder.Configuration);

        builder.Services.AddMassTransitDefaults(configure =>
        {
            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.AddRabbitMqHost(context);
            });
        });

        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
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

            options.AddPolicy("ManageUserBalance", policy =>
                policy.Requirements.Add(new ManageUserBalanceRequirement()));
        });

        builder.Services.AddSingleton<IAuthorizationHandler, RequireUserIdHandler>();
        builder.Services.AddSingleton<IAuthorizationHandler, ManageUserBalanceHandler>();

        builder.Services.AddValidatorsFromAssemblyContaining<GetUserBalanceByPeriodRequestValidator>();

        return builder;
    }
}
