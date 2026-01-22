using TreasuryFlow.Domain.Transactions.Entities;
using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Application.Transactions.Inputs;

public record CreateTransactionInput(
    Guid UserId,
    decimal Amount,
    ETransactionType TransactionType)
{

    public TransactionEntity ToEntity() =>
        new()
        {
            UserId = UserId,
            TransactionType = TransactionType,
            Amount = Amount
        };
};
