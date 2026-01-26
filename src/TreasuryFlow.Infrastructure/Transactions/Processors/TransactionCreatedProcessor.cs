using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Application.Transactions.Processors.Interfaces;
using TreasuryFlow.Application.UserBalances.Services.Interfaces;
using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Infrastructure.Transactions.Processors;

public class TransactionCreatedProcessor(
    ITreasuryFlowDbContext treasuryFlowDbContext,
    IUserBalanceService userBalanceService) : ITransactionCreatedProcessor
{
    public async Task DoAsync(
        Guid userId,
        Guid transactionId,
        CancellationToken cancellationToken)
    {
        var transaction = await treasuryFlowDbContext.Transactions.FirstOrDefaultAsync(f => f.Id == transactionId,
            cancellationToken)
            ?? throw new InvalidOperationException($"Transaction with id {transactionId} not found.");

        if (transaction.Status != ETransactionStatus.Pending)
            return;

        var user = await treasuryFlowDbContext.Users.FirstOrDefaultAsync(f => f.Id == userId, cancellationToken)
            ?? throw new InvalidOperationException($"User with id {userId} not found.");

        var transactionDate = transaction.GetFormatDateCreated();

        var userBalance = await userBalanceService.GetOrCreateUserBalanceAsync(userId, transactionDate);

        await userBalanceService.UpdateUserBalanceAndTransactionAsync(userBalance, transaction);

    }
}
