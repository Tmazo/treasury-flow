using TreasuryFlow.Domain.OwnerBalance.Entities;
using TreasuryFlow.Domain.Transactions.Entities;

namespace TreasuryFlow.Application.OwnerBalances.Services.Interfaces
{
    public interface IOwnerBalanceService
    {
        Task<OwnerBalanceEntity> GetOrCreateOwnerBalanceAsync(
            Guid ownerId,
            DateOnly date);

        Task UpdateOwnerBalanceAndTransactionAsync(
                    OwnerBalanceEntity ownerBalance,
                    TransactionEntity transaction);
    }
}
