using TreasuryFlow.Domain.Shared.Entities;
using TreasuryFlow.Domain.Transactions.Enums;
using TreasuryFlow.Domain.Transactions.Events;

namespace TreasuryFlow.Domain.Transactions.Entities;

public class TransactionEntity : BaseEntity
{
    public Guid UserId { get; private set; }
    public decimal Amount { get; private set; }
    public ETransactionType TransactionType { get; private set; }

    public TransactionEntity(
        Guid userId,
        decimal amount,
        ETransactionType transactionType)
    {
        UserId = userId;
        Amount = amount;
        TransactionType = transactionType;
    }
}
