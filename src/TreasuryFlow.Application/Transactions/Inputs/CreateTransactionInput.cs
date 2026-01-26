using TreasuryFlow.Domain.Transactions.Entities;
using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Application.Transactions.Inputs;

public record CreateTransactionInput(
    Guid UserId,
    decimal Amount,
    ETransactionType Type)
{

    public TransactionEntity ToEntity() =>
        new()
        {
            UserId = UserId,
            Type = Type,
            Amount = Amount
        };
};
