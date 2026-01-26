using TreasuryFlow.Domain.Owner.Entities;
using TreasuryFlow.Domain.Shared.Entities;
using TreasuryFlow.Domain.Transactions.Enums;
using TreasuryFlow.Domain.Transactions.Events;

namespace TreasuryFlow.Domain.Transactions.Entities;

public class TransactionEntity : BaseEntity
{
    public Guid OwnerId { get; set; }
    public decimal Amount { get; set; }
    public ETransactionType Type { get; set; }
    public ETransactionStatus Status { get; set; } = ETransactionStatus.Pending;

    public OwnerEntity Owner { get; set; }

    public TransactionCreatedEvent ToEvent() =>
        new()
        {
            Id = Id,
            OwnerId = OwnerId,
            Type = Type,
            Amount = Amount
        };

    public DateOnly GetFormatDateCreated() => DateOnly.FromDateTime(CreatedAt.UtcDateTime);
}
