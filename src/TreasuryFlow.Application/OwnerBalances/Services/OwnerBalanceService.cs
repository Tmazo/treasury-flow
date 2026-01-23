using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Application.OwnerBalances.Services.Interfaces;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Domain.OwnerBalance.Entities;
using TreasuryFlow.Domain.Transactions.Entities;
using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Application.OwnerBalances.Services
{
    public class OwnerBalanceService(
        ITreasuryFlowDbContext treasuryFlowDbContext) : IOwnerBalanceService
    {
        public async Task<OwnerBalanceEntity> GetOrCreateOwnerBalanceAsync(
            Guid ownerId,
            DateOnly date)
        {
            var ownerBalance = await treasuryFlowDbContext.OwnerBalances
                .FirstOrDefaultAsync(f =>
                f.OwnerId == ownerId &&
                f.Date == date);

            if (ownerBalance is null)
            {
                ownerBalance = new OwnerBalanceEntity
                {
                    OwnerId = ownerId,
                    Date = date
                };

                await treasuryFlowDbContext.OwnerBalances.AddAsync(ownerBalance);
                await treasuryFlowDbContext.SaveChangesAsync();
            }

            return ownerBalance;
        }

        public async Task UpdateOwnerBalanceAndTransactionAsync(
            OwnerBalanceEntity ownerBalance,
            TransactionEntity transaction)
        {
            var lastOwnerBalance = await treasuryFlowDbContext.OwnerBalances
                    .Where(f => f.OwnerId == ownerBalance.OwnerId)
                    .OrderByDescending(f => f.Date)
                    .Select(f => new { f.TotalBalance })
                    .FirstOrDefaultAsync();

            var previousTotalBalance = lastOwnerBalance?.TotalBalance ?? 0m;
            var date = DateOnly.FromDateTime(transaction.CreatedAt.UtcDateTime);

            switch (transaction.Type)
            {
                case ETransactionType.Input:
                    ownerBalance.InputAmount += transaction.Amount;
                    break;
                case ETransactionType.Output:
                    ownerBalance.OutputAmount -= transaction.Amount;
                    break;
                default:
                    throw new NotImplementedException($"Transaction type {transaction.Type} not implemented.");
            }

            ownerBalance.ApplyDailyBalance(previousTotalBalance);
            transaction.Status = ETransactionStatus.Processed;

            await treasuryFlowDbContext.SaveChangesAsync();
        }
    }
}
