using Microsoft.EntityFrameworkCore;
using TreasuryFlow.Application.Shared.Communications.Interfaces;
using TreasuryFlow.Application.Shared.Data.Interfaces;
using TreasuryFlow.Application.Shared.Exceptions;
using TreasuryFlow.Application.Transactions.Inputs;
using TreasuryFlow.Application.Transactions.Outputs;
using TreasuryFlow.Application.Transactions.Services.Interfaces;

namespace TreasuryFlow.Application.Transactions.Services;

public class TransactionService(ITreasuryFlowDbContext treasuryFlowDbContext, IEventPublisher eventPublisher) : ITransactionService
{
    public async Task<CreateTransactionResponse> CreateAsync(
        CreateTransactionInput input,
        CancellationToken cancellationToken)
    {
        var isValidUser =
            await treasuryFlowDbContext.Users.AnyAsync(s => s.Id == input.UserId, cancellationToken);

        if (!isValidUser)
            throw new ForbiddenAccessException("User is not allowed.");

        var entity = input.ToEntity();

        await treasuryFlowDbContext.Transactions.AddAsync(
            entity,
            cancellationToken);

        await treasuryFlowDbContext.SaveChangesAsync(cancellationToken);

        await eventPublisher.SendAsRawJsonAsync(
            entity.ToEvent(),
            cancellationToken);

        return new CreateTransactionResponse(entity.Id);
    }
}
