using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TreasuryFlow.Application.Shared.Communications.Interfaces;

namespace TreasuryFlow.Infrastructure.Shared.Communications;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransitDefaults(this IServiceCollection services,
        Action<IBusRegistrationConfigurator> configure)
    {
        services.AddScoped<IEventPublisher, EventPublisher>();

        services.ConfigureOptions<ConfigureRabbitMqOptions>();

        return services.AddMassTransit((x) =>
        {
            configure(x);
        });
    }

    public static void AddRabbitMqHost(this IRabbitMqBusFactoryConfigurator configurator, IBusRegistrationContext context)
    {
        var rabbitMqOptions = context.GetRequiredService<IOptions<RabbitMqOptions>>().Value;

        configurator.Host(rabbitMqOptions.ConnectionString);
        configurator.UseRawJsonSerializer();
        configurator.ConfigureJsonSerializerOptions(jsonSerializerSettings =>
        {
            jsonSerializerSettings.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
            return jsonSerializerSettings;
        });
    }
}
