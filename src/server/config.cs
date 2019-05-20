using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace server
{
    public class Config
    {
        public static IEnumerable<Client> GetClients()
        {
            return new Client[]
            {
                new Client
                {
                    ClientId = "identity_server_client",
                    ClientName = "Identity Server (Client)",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    RedirectUris = { "http://localhost:8001/signin-oidc" },
                    AllowedScopes = { "openid", "email", "office" }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),
                new IdentityResources.Profile(),
                new IdentityResource
                {
                    Name = "office",
                    UserClaims = { "office_number" }
                }
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new ApiResource[]
            {

            };
        }
    }
}
