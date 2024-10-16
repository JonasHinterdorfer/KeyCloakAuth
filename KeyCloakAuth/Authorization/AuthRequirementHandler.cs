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
        catch (Exception e)
        {
            var message = $"Error while handling {nameof(AuthRequirement)}: {e.Message}";
            context.Fail();
        }
        return Task.CompletedTask;
    }
    
    private void HandleUserRequirements(User user, AuthorizationHandlerContext context, AuthRequirement requirement)
    {
        var success = requirement.Roles switch
        {
            Roles.Admin when user.Roles.Contains(Roles.Admin) => true,
            Roles.User when user.Roles.Contains(Roles.User) => true,
            Roles.test when user.Roles.Contains(Roles.test) => true,
            _ => false
        };
        if (success)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}