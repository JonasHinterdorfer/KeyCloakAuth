using System.Collections.Frozen;
using System.Security.Claims;
using KeyCloakAuth.Models;

namespace KeyCloakAuth.Authentication;

internal static class UserProvider
{
    private static FrozenSet<string> _relevantClaimsSet = CreateRelevantClaimsSet();
    
    public static User GetUserInformation(this ClaimsPrincipal principal)
    {
        var claims = principal.Claims;
        var relevantClaims = claims
                             .Select(c => (Claim: c, Type: c.Type.Trim()))
                             .Where(t => _relevantClaimsSet.Contains(t.Type))
                             .ToDictionary(t => t.Type, t => t.Claim, StringComparer.OrdinalIgnoreCase);
        throw new NotImplementedException();
    }
    
    
    private static FrozenSet<string> CreateRelevantClaimsSet()
    {
        IEnumerable<string> claims =
        [
            Claims.EmailVerifiedClaimType,
            Claims.Username,
            Claims.RealmRoles
        ];

        return claims.ToFrozenSet();
    }
}