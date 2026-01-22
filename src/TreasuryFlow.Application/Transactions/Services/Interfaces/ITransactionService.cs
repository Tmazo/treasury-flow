using TreasuryFlow.Application.Transactions.Inputs;
using TreasuryFlow.Application.Transactions.Outputs;

namespace TreasuryFlow.Application.Transactions.Services.Interfaces;

public interface ITransactionService
{
    Task<CreateTransactionResponse> CreateAsync(
        CreateTransactionInput request,
        CancellationToken cancellationToken);
}
