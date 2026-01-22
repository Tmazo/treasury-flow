using MassTransit;
using TreasuryFlow.Domain.Transactions.Events;

namespace TreasuryFlow.Consumer.Consumers
{
    public class ProcessTransactionOnCreatedConsumer : IConsumer<TransactionCreatedEvent>
    {
        public Task Consume(ConsumeContext<TransactionCreatedEvent> context)
        {
            throw new NotImplementedException();
        }
    }

    public static class ReceiveEndpointConfiguration
    {
        public static void AddProcessTransactionOnCreatedConsumer(
            this IRabbitMqBusFactoryConfigurator configurator,
            IBusRegistrationContext context)
        {
            configurator.ReceiveEndpoint(nameof(ProcessTransactionOnCreatedConsumer), ep =>
            {
                ep.ConfigureConsumer<ProcessTransactionOnCreatedConsumer>(context);

                ep.Bind(nameof(TransactionCreatedEvent), config =>
                {
                    config.ExchangeType = RabbitMQ.Client.ExchangeType.Fanout;
                    config.Durable = true;
                });
            });
        }
    }
}
