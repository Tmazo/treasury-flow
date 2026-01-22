using MassTransit;
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

        return builder;
    }
}
