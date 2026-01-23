using MassTransit;
using TreasuryFlow.Application.Transactions.Processors.Interfaces;
using TreasuryFlow.Domain.Transactions.Events;

namespace TreasuryFlow.Consumer.Consumers
{
    public class ProcessTransactionOnCreatedConsumer(
        ITransactionCreatedProcessor transactionCreatedProcessor,
        ILogger<ProcessTransactionOnCreatedConsumer> _logger) 
        : IConsumer<TransactionCreatedEvent>
    {
        public async Task Consume(ConsumeContext<TransactionCreatedEvent> context)
        {
            _logger.LogInformation(
                "Started processing TransactionCreatedEvent. TransactionId: {TransactionId}",
                context.Message.Id);

            var ownerId = context.Message.OwnerId;
            var transactionId = context.Message.Id;

            await transactionCreatedProcessor.DoAsync(ownerId: ownerId, transactionId: transactionId, context.CancellationToken);

            _logger.LogInformation(
                "Completed processing TransactionCreatedEvent. TransactionId: {TransactionId}",
                context.Message.Id);
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
