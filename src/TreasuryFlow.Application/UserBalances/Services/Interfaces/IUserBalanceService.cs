using TreasuryFlow.Application.UserBalances.Inputs;
using TreasuryFlow.Application.UserBalances.Outputs;
using TreasuryFlow.Domain.Transactions.Entities;
using TreasuryFlow.Domain.UserBalance.Entities;

namespace TreasuryFlow.Application.UserBalances.Services.Interfaces;

public interface IUserBalanceService
{
    Task<UserBalanceEntity> GetOrCreateUserBalanceAsync(
        Guid userId,
        DateOnly date);

    Task UpdateUserBalanceAndTransactionAsync(
                UserBalanceEntity userBalance,
                TransactionEntity transaction);

    Task<IEnumerable<GetUserBalancesByPeriodOutput>> GetUserBalancesByPeriodAsync(
        GetUserBalancesByPeriodInput input,
        CancellationToken cancellationToken);
}