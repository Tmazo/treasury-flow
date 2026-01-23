namespace TreasuryFlow.Application.Transactions.Processors.Interfaces;

public interface ITransactionCreatedProcessor
{
    Task DoAsync(
        Guid ownerId,
        Guid transactionId,
        CancellationToken cancellationToken);
}
