using TreasuryFlow.Domain.Shared.Entities;
using TreasuryFlow.Domain.Transactions.Enums;
using TreasuryFlow.Domain.Transactions.Events;

namespace TreasuryFlow.Domain.Transactions.Entities;

public class TransactionEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public ETransactionType TransactionType { get; set; }

    public TransactionCreatedEvent ToCreatedEvent() =>
        new()
        {
            UserId = UserId,
            TransactionType = TransactionType,
            Amount = Amount
        };
}
