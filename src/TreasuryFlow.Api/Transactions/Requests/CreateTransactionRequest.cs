using TreasuryFlow.Application.Transactions.Inputs;
using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Api.Transactions.Requests;

public class CreateTransactionRequest
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public ETransactionType TransactionType { get; set; }

    public CreateTransactionInput ToInput() =>
        new(UserId, Amount, TransactionType);
}