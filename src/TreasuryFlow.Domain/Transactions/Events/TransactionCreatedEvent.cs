using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Domain.Transactions.Events;

public class TransactionCreatedEvent
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public decimal Amount { get; set; }
    public ETransactionType Type { get; set; }
}