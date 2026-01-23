using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Domain.Owner.Entities;
using TreasuryFlow.Domain.OwnerBalance.Entities;
using TreasuryFlow.Domain.Transactions.Entities;

namespace TreasuryFlow.Infrastructure.Shared.Data;

public class TreasuryFlowDbContext : DbContext, ITreasuryFlowDbContext
{
    public DbSet<TransactionEntity> Transactions { get; set; }
    public DbSet<OwnerEntity> Owners { get; set; }
    public DbSet<OwnerBalanceEntity> OwnerBalances { get; set; }

    public TreasuryFlowDbContext(DbContextOptions<TreasuryFlowDbContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TreasuryFlowDbContext).Assembly);
    }
}
