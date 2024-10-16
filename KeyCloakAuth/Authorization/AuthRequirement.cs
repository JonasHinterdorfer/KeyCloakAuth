using Microsoft.AspNetCore.Authorization;

namespace KeyCloakAuth.Authorization;

public readonly struct AuthRequirement(Role role) : IAuthorizationRequirement
{
    public Role Role { get; } = role;
}