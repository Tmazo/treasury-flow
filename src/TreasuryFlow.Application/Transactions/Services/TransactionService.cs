using TreasuryFlow.Application.Transactions.Inputs;
using TreasuryFlow.Application.Transactions.Outputs;
using TreasuryFlow.Application.Transactions.Services.Interfaces;

namespace TreasuryFlow.Application.Transactions.Services;

public class TransactionService : ITransactionService
{
    public Task<CreateTransactionResponse> CreateAsync(
        CreateTransactionInput request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
