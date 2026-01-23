using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Application.OwnerBalances.Services.Interfaces;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Application.Transactions.Processors.Interfaces;
using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Infrastructure.Transactions.Processors;

public class TransactionCreatedProcessor(
    ITreasuryFlowDbContext treasuryFlowDbContext,
    IOwnerBalanceService ownerBalanceService) : ITransactionCreatedProcessor
{
    public async Task DoAsync(
        Guid ownerId,
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        var transaction = await treasuryFlowDbContext.Transactions.FirstOrDefaultAsync(f => f.Id == transactionId,
            cancellationToken)
            ?? throw new InvalidOperationException($"Transaction with id {transactionId} not found.");

        if (transaction.Status != ETransactionStatus.Pending)
            return;

        var owner = await treasuryFlowDbContext.Owners.FirstOrDefaultAsync(f => f.Id == ownerId, cancellationToken)
            ?? throw new InvalidOperationException($"Owner with id {ownerId} not found.");

        var transactionDate = transaction.GetFormatDateCreated();

        var ownerBalance = await ownerBalanceService.GetOrCreateOwnerBalanceAsync(ownerId, transactionDate);

        await ownerBalanceService.UpdateOwnerBalanceAndTransactionAsync(ownerBalance, transaction);

    }
}
