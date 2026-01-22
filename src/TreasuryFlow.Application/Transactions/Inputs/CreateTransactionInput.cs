using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Application.Transactions.Inputs;

public record CreateTransactionInput(
    Guid TransactionId,
    decimal Amount,
    ETransactionType TransactionType);
