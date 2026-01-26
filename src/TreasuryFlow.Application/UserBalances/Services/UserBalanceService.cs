using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Application.UserBalances.Services.Interfaces;
using TreasuryFlow.Domain.Transactions.Entities;
using TreasuryFlow.Domain.Transactions.Enums;
using TreasuryFlow.Domain.UserBalance.Entities;

namespace TreasuryFlow.Application.UserBalances.Services
{
    public class UserBalanceService(
        ITreasuryFlowDbContext treasuryFlowDbContext) : IUserBalanceService
    {
        public async Task<UserBalanceEntity> GetOrCreateUserBalanceAsync(
            Guid userId,
            DateOnly date)
        {
            var userBalance = await treasuryFlowDbContext.UserBalances
                .FirstOrDefaultAsync(f =>
                f.UserId == userId &&
                f.Date == date);

            if (userBalance is null)
            {
                userBalance = new UserBalanceEntity
                {
                    UserId = userId,
                    Date = date
                };

                await treasuryFlowDbContext.UserBalances.AddAsync(userBalance);
                await treasuryFlowDbContext.SaveChangesAsync();
            }

            return userBalance;
        }

        public async Task UpdateUserBalanceAndTransactionAsync(
            UserBalanceEntity userBalance,
            TransactionEntity transaction)
        {
            var lastUserBalance = await treasuryFlowDbContext.UserBalances
                    .Where(f => f.UserId == userBalance.UserId)
                    .OrderByDescending(f => f.Date)
                    .Select(f => new { f.TotalBalance })
                    .FirstOrDefaultAsync();

            var previousTotalBalance = lastUserBalance?.TotalBalance ?? 0m;
            var date = DateOnly.FromDateTime(transaction.CreatedAt.UtcDateTime);

            switch (transaction.Type)
            {
                case ETransactionType.Input:
                    userBalance.InputAmount += transaction.Amount;
                    break;
                case ETransactionType.Output:
                    userBalance.OutputAmount -= transaction.Amount;
                    break;
                default:
                    throw new NotImplementedException($"Transaction type {transaction.Type} not implemented.");
            }

            userBalance.ApplyDailyBalance(previousTotalBalance);
            transaction.Status = ETransactionStatus.Processed;

            await treasuryFlowDbContext.SaveChangesAsync();
        }
    }
}
