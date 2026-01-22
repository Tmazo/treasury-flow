using Microsoft.AspNetCore.Mvc;
using TreasuryFlow.Api.Transactions.Requests;
using TreasuryFlow.Application.Transactions.Services.Interfaces;

namespace TreasuryFlow.Api.Transactions;

//[Authorize]
[ApiController]
[Route("api/[controller]")]

public class TransactionsController(ITransactionService service) : ControllerBase
{
    /// <summary>
    /// Cria uma nova transação financeira.
    /// </summary>
    /// <param name="request">Dados necessários para criação da transação.</param>
    /// <returns>Dados da transação criada.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateAsync(
            request.ToInput(),
            cancellationToken);

        return Ok(result);
    }
}
