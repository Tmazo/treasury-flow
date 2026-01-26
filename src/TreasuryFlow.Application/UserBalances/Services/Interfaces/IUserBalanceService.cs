using TreasuryFlow.Domain.UserBalance.Entities;
using TreasuryFlow.Domain.Transactions.Entities;

namespace TreasuryFlow.Application.UserBalances.Services.Interfaces
{
    public interface IUserBalanceService
    {
        Task<UserBalanceEntity> GetOrCreateUserBalanceAsync(
            Guid userId,
            DateOnly date);

        Task UpdateUserBalanceAndTransactionAsync(
                    UserBalanceEntity userBalance,
                    TransactionEntity transaction);
    }
}
