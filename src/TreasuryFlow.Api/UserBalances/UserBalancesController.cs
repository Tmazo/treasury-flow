using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TreasuryFlow.Api.UserBalances.Requests;
using TreasuryFlow.Application.UserBalances.Services.Interfaces;

namespace TreasuryFlow.Api.UserBalances;

[Authorize(Policy = "RequireUserId")]
[Authorize(Policy = "ManageUserBalance")]
[ApiController]
[Route("api/[controller]")]
public class UserBalancesController(
    IUserBalanceService userBalanceService,
    IValidator<GetUserBalanceByPeriodRequest> validator) : Controller
{

    /// <summary>
    /// Busca o balance diário.
    /// </summary>
    /// <param name="request">Dados necessários para busca do balance diário.</param>
    /// <returns>Dados do balance diário.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAsync(
        [FromQuery] GetUserBalanceByPeriodRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(
            request,
            cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.ToDictionary());

        var result = await userBalanceService.GetUserBalancesByPeriodAsync(
            request.ToInput(),
            cancellationToken);

        return Ok(result);
    }
}
