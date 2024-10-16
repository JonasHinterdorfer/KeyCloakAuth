using KeyCloakAuth.Authentication;
using KeyCloakAuth.Models;
using Microsoft.AspNetCore.Authorization;

namespace KeyCloakAuth.Authorization;

public sealed class AuthRequirementHandler : AuthorizationHandler<AuthRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
    {
        try
        {
            var user = context.User.GetUserInformation();
            HandleUserRequirements(user, context, requirement);
        }
        catch (Exception)
        {
            context.Fail();
        }
        return Task.CompletedTask;
    }

    private void HandleUserRequirements(User user, AuthorizationHandlerContext context, AuthRequirement requirement)
    {
        if (user.Roles.Contains(requirement.Role))
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}