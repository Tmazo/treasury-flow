using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TreasuryFlow.Domain.Shared.Enums;

namespace TreasuryFlow.Api.Auth.Requirements.Handlers;

public class ManageUserBalanceHandler : AuthorizationHandler<ManageUserBalanceRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ManageUserBalanceRequirement requirement)
    {
        var role = context.User.FindFirstValue(ClaimTypes.Role);

        if (role == EPermissionRole.Admin.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
