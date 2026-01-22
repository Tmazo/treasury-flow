using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Domain.Transactions.Entities;

namespace TreasuryFlow.Application.Shared.Data.Interfaces;

public interface ITreasuryFlowDbContext
{
    public DbSet<TransactionEntity> Transactions { get; set; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
