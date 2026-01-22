using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Application.Transactions.Outputs;

public record CreateTransactionResponse(
    Guid TransactionId,
    decimal Amount,
    ETransactionType Type,
    DateTimeOffset CreatedAt);
