using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Application.Shared.Interfaces;
using TreasuryFlow.Domain.Transactions.Entities;

namespace TreasuryFlow.Infrastructure.Shared.Data;

public class TreasuryFlowDbContext : DbContext, ITreasuryFlowDbContext
{
    public DbSet<TransactionEntity> Transactions { get; set; }

    public TreasuryFlowDbContext(DbContextOptions<TreasuryFlowDbContext> options)
        : base(options)
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TreasuryFlowDbContext).Assembly);
    }
}
