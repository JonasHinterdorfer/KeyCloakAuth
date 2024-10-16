using System;
using System.Linq;
using System.Text.Json.Serialization;
using KeyCloakAuth.Authorization;
using OneOf;
using OneOf.Types;

namespace KeyCloakAuth.Models
{
    public class RealmAccess
    {
        [JsonPropertyName("roles")]
        public string[] Roles { get; init; }

        public RealmAccess(string[] roles)
        {
            Roles = roles;
        }

        public Roles[] GetRoles()
        {
            return Roles.Select(x =>
            {
                bool isSuccess = Enum.TryParse<Roles>(x, out var role);
                if (isSuccess)
                {
                    return OneOf<Roles, None>.FromT0(role);
                }

                return OneOf<Roles, None>.FromT1(new None());
            }).Where(x => x.IsT0).Select(x => x.AsT0).ToArray();
        }
    }
}