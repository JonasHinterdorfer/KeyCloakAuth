using Microsoft.AspNetCore.Authorization;

namespace KeyCloakAuth.Authorization;

public readonly struct AuthRequirement(Roles roles) : IAuthorizationRequirement
{
    public Roles Roles { get; } = roles;
}