using System.Security.Claims;
using KeyCloakAuth.Models;
using System.Text.Json;

namespace KeyCloakAuth.Authentication;

internal static class UserProvider
{
    private static readonly HashSet<string> RelevantClaimsSet = new()
    {
        Claims.EmailVerifiedClaimType,
        Claims.Username,
        Claims.RealmAccess
    };

    public static User GetUserInformation(this ClaimsPrincipal principal)
    {
        var relevantClaims = principal.Claims
            .Where(c => RelevantClaimsSet.Contains(c.Type.Trim()))
            .ToDictionary(c => c.Type.Trim(), c => c, StringComparer.OrdinalIgnoreCase);

        var userName = relevantClaims[Claims.Username].Value;
        var emailConfirmed = bool.Parse(relevantClaims[Claims.EmailVerifiedClaimType].Value);
        var realmAccess = JsonSerializer.Deserialize<RealmAccess>(relevantClaims[Claims.RealmAccess].Value)
                          ?? throw new InvalidOperationException("RealmAccess claim is missing");

        return new User(userName, emailConfirmed, realmAccess.GetRoles());
    }
}