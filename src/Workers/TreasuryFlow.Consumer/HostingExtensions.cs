using MassTransit;
using Microsoft.AspNetCore.Builder;
using TreasuryFlow.Application;
using TreasuryFlow.Consumer.Consumers;
using TreasuryFlow.Infrastructure;
using TreasuryFlow.Infrastructure.Shared.Communications;

namespace TreasuryFlow.Consumer;

public static class HostingExtensions
{
    public static WebApplicationBuilder ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.AddServiceDefaults();

        builder.Services.AddDatabase(builder.Configuration);
        builder.Services.AddServices();
        builder.Services.AddProcessors();

        builder.Services.AddMassTransitDefaults(configure =>
        {
            configure.AddConsumers(typeof(ProcessTransactionOnCreatedConsumer).Assembly);

            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.AddRabbitMqHost(context);
                cfg.AddProcessTransactionOnCreatedConsumer(context);
            });
        });

        return builder;
    }
}