using KeyCloakAuth.Authorization;

namespace KeyCloakAuth.Models;

public sealed class User(string userName, bool emailConfirmed, Roles[] roles)
{
    public Roles[] Roles { get;} = roles;
    public bool EmailConfirmed { get; } = emailConfirmed;
    public string UserName { get; } = userName;
}