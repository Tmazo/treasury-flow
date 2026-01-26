namespace TreasuryFlow.Application.Transactions.Processors.Interfaces;

public interface ITransactionCreatedProcessor
{
    Task DoAsync(
        Guid userId,
        Guid transactionId,
        CancellationToken cancellationToken);
}
