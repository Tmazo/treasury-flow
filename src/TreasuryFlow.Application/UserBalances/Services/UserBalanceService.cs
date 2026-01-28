using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Application.Shared.Caching.Interfaces;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Application.UserBalances.Inputs;
using TreasuryFlow.Application.UserBalances.Outputs;
using TreasuryFlow.Application.UserBalances.Services.Interfaces;
using TreasuryFlow.Domain.Transactions.Entities;
using TreasuryFlow.Domain.Transactions.Enums;
using TreasuryFlow.Domain.UserBalance.Entities;

namespace TreasuryFlow.Application.UserBalances.Services;

public class UserBalanceService(
    ITreasuryFlowDbContext treasuryFlowDbContext,
    ICacheService cache) : IUserBalanceService
{
    public async Task<UserBalanceEntity> GetOrCreateUserBalanceAsync(
        Guid userId,
        DateOnly date)
    {
        var userBalance = await treasuryFlowDbContext.UserBalances
            .AsTracking()
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

    public async Task<IEnumerable<GetUserBalancesByPeriodOutput>> GetUserBalancesByPeriodAsync(
        GetUserBalancesByPeriodInput input,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"user-balances:{input.InitialPeriod}:{input.FinalPeriod}";

        var cached = await cache.GetAsync<IEnumerable<GetUserBalancesByPeriodOutput>>(
            cacheKey,
            cancellationToken);

        if (cached is not null)
            return cached;

        var data = await treasuryFlowDbContext.UserBalances
            .AsNoTracking()
            .Where(ub =>
                ub.Date >= input.InitialPeriod &&
                ub.Date <= input.FinalPeriod)
            .ToListAsync(cancellationToken);

        var result = data
            .GroupBy(g => g.Date)
            .OrderBy(g => g.Key.DayNumber)
            .Select(dateGroup => new GetUserBalancesByPeriodOutput
            {
                Date = dateGroup.Key,

                UserBalances = dateGroup
                    .GroupBy(ub => ub.UserId)
                    .Select(userGroup => new GetUserBalancesByPeriodDto
                    {
                        UserId = userGroup.Key,
                        Date = dateGroup.Key,
                        InputAmount = userGroup.Sum(x => x.InputAmount),
                        OutputAmount = userGroup.Sum(x => x.OutputAmount),
                        DailyBalance = userGroup.Sum(x => x.DailyBalance),
                        TotalBalance = userGroup.Sum(x => x.TotalBalance)
                    })
                    .ToList(),

                DateTotalBalance = dateGroup
                    .GroupBy(ub => ub.UserId)
                    .Sum(g => g.Sum(x => x.TotalBalance))
            })
            .ToList();

        var ttl = ResolveTtl(input);
        await cache.SetAsync(cacheKey, result, ttl, cancellationToken);

        return result;
    }

    private static TimeSpan ResolveTtl(GetUserBalancesByPeriodInput input)
    {
        // dia atual muda o tempo todo
        if (input.FinalPeriod == DateOnly.FromDateTime(DateTime.UtcNow))
            return TimeSpan.FromSeconds(30);

        // dias passados são praticamente imutáveis
        return TimeSpan.FromMinutes(10);
    }
}