using System.Collections.Frozen;
using System.Security.Claims;
using KeyCloakAuth.Models;
using System.Text.Json;
using KeyCloakAuth.Authorization;

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

        var userName = relevantClaims[Claims.Username].Value;
        var emailConfirmed = bool.Parse(relevantClaims[Claims.EmailVerifiedClaimType].Value);
        RealmAccess? realmAccess = JsonSerializer.Deserialize<RealmAccess>(relevantClaims[Claims.RealmAccess].Value);
        
        //TODO: ADD validation
        if(realmAccess is null)
        {
            throw new InvalidOperationException("RealmAccess claim is missing");
        }
        
        return new User(userName, emailConfirmed, realmAccess.GetRoles());
    }


    private static FrozenSet<string> CreateRelevantClaimsSet()
    {
        IEnumerable<string> claims =
        [
            Claims.EmailVerifiedClaimType,
            Claims.Username,
            Claims.RealmAccess
        ];

        return claims.ToFrozenSet();
    }
}