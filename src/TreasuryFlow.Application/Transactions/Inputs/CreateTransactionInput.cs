using TreasuryFlow.Domain.Transactions.Entities;
using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Application.Transactions.Inputs;

public record CreateTransactionInput(
    Guid OwnerId,
    decimal Amount,
    ETransactionType Type)
{

    public TransactionEntity ToEntity() =>
        new()
        {
            OwnerId = OwnerId,
            Type = Type,
            Amount = Amount
        };
};
