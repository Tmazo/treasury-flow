using Microsoft.AspNetCore.Authorization;
using TreasuryFlow.Api.Auth.Requirements;
using TreasuryFlow.Application.Shared.Extensions;

namespace TreasuryFlow.Api.Auth.Requirements.Handlers
{
    public class RequireUserIdHandler
        : AuthorizationHandler<RequireUserIdRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            RequireUserIdRequirement requirement)
        {
            var userId = context.User.GetUserIdAsNullableGuid();

            if (userId is not null)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }

}
