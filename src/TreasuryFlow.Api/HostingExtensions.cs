using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;
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

        //builder.Services.ConfigureEndpoints();

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
                Description = "Enter the JWT token in the format: Bearer {your_token}"
            });

            //options.SwaggerDoc("v1", new()
            //{
            //    Title = "TreasuryFlow API",
            //    Version = "v1"
            //});

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

        builder.Services.AddInfrastructureServices();
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

                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsync("""                          
                            {"error": "User not authenticated or invalid token."}                            
                            """);
                    },

                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        context.Response.ContentType = "application/json";

                        return context.Response.WriteAsync("""                            
                            {"error": "The user does not have permission to perform this operation."}                            
                            """);
                    }
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

    //private static IServiceCollection ConfigureEndpoints(this IServiceCollection services)
    //{
    //    services.AddEndpointsApiExplorer();
    //    services.AddSwaggerGen(options =>
    //    {
    //        options.SwaggerDoc("v1", new OpenApiInfo
    //        {
    //            Version = "v1",
    //            Title = "CashFlow Api",
    //            Description = "Service that manages Merchants Cash Flows",
    //        });

    //        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //        {
    //            Name = "Authorization",
    //            Type = SecuritySchemeType.ApiKey,
    //            Scheme = "Bearer",
    //            BearerFormat = "JWT",
    //            In = ParameterLocation.Header,
    //            Description = "Insira o token JWT desta maneira: Bearer {seu token}"
    //        });

    //        options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //            {
    //                {
    //                    new OpenApiSecurityScheme
    //                    {
    //                        Reference = new OpenApiReference
    //                        {
    //                            Type = ReferenceType.SecurityScheme,
    //                            Id = "Bearer"
    //                        }
    //                    },
    //                    Array.Empty<string>()
    //                }
    //            });
    //    });

    //    services.ConfigureHttpJsonOptions(options =>
    //    {
    //        options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    //    });

    //    return services;
    //}
}
