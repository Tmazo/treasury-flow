using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Domain.Transactions.Events;

public class TransactionCreatedEvent
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public ETransactionType TransactionType { get; set; }
}