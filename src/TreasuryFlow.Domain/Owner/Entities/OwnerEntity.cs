using TreasuryFlow.Domain.OwnerBalance.Entities;
using TreasuryFlow.Domain.Shared.Entities;
using TreasuryFlow.Domain.Transactions.Entities;

namespace TreasuryFlow.Domain.Owner.Entities;

public class OwnerEntity : BaseEntity
{
    public required string Name { get; set; }
    public IEnumerable<OwnerBalanceEntity>? OwnerBalances { get; set; }
    public IEnumerable<TransactionEntity>? Transactions { get; set; }
}
