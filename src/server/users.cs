using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Test;
using System.Security.Claims;
using IdentityModel;

namespace server
{
    public class Users
    {
        public static List<TestUser> _users = new List<TestUser>
        {
            new TestUser{SubjectId = "818727", Username = "Alice", Password = "alice_password",
                Claims = {
                    new Claim("office_number", "100"),
                    new Claim(JwtClaimTypes.Name, "Alice Smith"),
                    new Claim(JwtClaimTypes.GivenName, "Alice"),
                    new Claim(JwtClaimTypes.FamilyName, "Smith"),
                    new Claim(JwtClaimTypes.Email, "alice.smith@email.com"),
                    new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Email),
                    new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                    new Claim(JwtClaimTypes.Address, @"{'street_address': '1000 Alice Address'}")

                }
            }
        }
    }
}
