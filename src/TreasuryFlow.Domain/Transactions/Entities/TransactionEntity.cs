using TreasuryFlow.Domain.Shared.Entities;
using TreasuryFlow.Domain.Transactions.Enums;
using TreasuryFlow.Domain.Transactions.Events;
using TreasuryFlow.Domain.User.Entities;

namespace TreasuryFlow.Domain.Transactions.Entities;

public class TransactionEntity : BaseEntity
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public ETransactionType Type { get; set; }
    public ETransactionStatus Status { get; set; } = ETransactionStatus.Pending;

    public UserEntity User { get; set; }

    public TransactionCreatedEvent ToEvent() =>
        new()
        {
            Id = Id,
            UserId = UserId,
            Type = Type,
            Amount = Amount
        };

    public DateOnly GetFormatDateCreated() => DateOnly.FromDateTime(CreatedAt.UtcDateTime);
}
