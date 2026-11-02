using Microsoft.AspNetCore.Authorization;

namespace StandardInteriors.Api.Controllers;

public sealed class PaymentVoidRequirement : IAuthorizationRequirement
{
}

public sealed class PaymentVoidHandler : AuthorizationHandler<PaymentVoidRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PaymentVoidRequirement requirement)
    {
        if (context.User.IsInRole("Admin") || context.User.HasClaim("permission", "payments.void"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
