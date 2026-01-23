using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Domain.Owner.Entities;
using TreasuryFlow.Domain.OwnerBalance.Entities;
using TreasuryFlow.Domain.Transactions.Entities;

namespace TreasuryFlow.Application.Shared.Data.Interfaces;

public interface ITreasuryFlowDbContext
{
    public DbSet<TransactionEntity> Transactions { get; set; }
    public DbSet<OwnerEntity> Owners { get; set; }
    public DbSet<OwnerBalanceEntity> OwnerBalances { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
