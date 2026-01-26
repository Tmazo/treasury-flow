using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Domain.User.Entities;
using TreasuryFlow.Domain.UserBalance.Entities;
using TreasuryFlow.Domain.Transactions.Entities;

namespace TreasuryFlow.Application.Shared.Data.Interfaces;

public interface ITreasuryFlowDbContext
{
    public DbSet<TransactionEntity> Transactions { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserBalanceEntity> UserBalances { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
