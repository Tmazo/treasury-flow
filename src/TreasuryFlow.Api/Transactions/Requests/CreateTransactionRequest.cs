using TreasuryFlow.Application.Transactions.Inputs;
using TreasuryFlow.Domain.Transactions.Enums;

namespace TreasuryFlow.Api.Transactions.Requests;

public class CreateTransactionRequest
{
    public decimal Amount { get; set; }
    public ETransactionType TransactionType { get; set; }

    public CreateTransactionInput ToInput(Guid UserId) =>
        new(UserId, Amount, TransactionType);
}