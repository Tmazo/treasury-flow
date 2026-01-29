using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreasuryFlow.Api.Transactions.Requests;
using TreasuryFlow.Application.Shared.Extensions;
using TreasuryFlow.Application.Transactions.Services.Interfaces;
using Asp.Versioning;

namespace TreasuryFlow.Api.Transactions;

[Authorize(Policy = "RequireUserId")]
//[ApiVersion("1.0")]
[Route("api/[controller]")]

public class TransactionsController(ITransactionService service,
    IValidator<CreateTransactionRequest> validator) : ControllerBase
{
    /// <summary>
    /// Cria uma nova transação financeira.
    /// </summary>
    /// <param name="request">Dados necessários para criação da transação.</param>
    /// <returns>Dados da transação criada.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = User.GetUserIdAsValidatedGuid();

        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.ToDictionary());

        var result = await service.CreateAsync(
            request.ToInput(userId),
            cancellationToken);

        return Ok(result);
    }
}
