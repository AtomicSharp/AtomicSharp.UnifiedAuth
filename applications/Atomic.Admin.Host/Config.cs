using System.Collections.Generic;
using IdentityServer4.Models;

namespace Atomic.Admin.Host
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new("weather"),
                new ApiScope("scope2")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new[]
            {
                new ApiResource("atomic-admin", "Administration of Atomic App")
                {
                    Scopes = { "weather" }
                }
            };


        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    ClientId = "admin-web",
                    ClientName = "Atomic.Admin.Web",
                    ClientSecrets = { new Secret("admin".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = true,

                    RedirectUris = { "https://localhost:8000/account/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:8000/account/signout-oidc" },

                    AllowOfflineAccess = true,
                    AllowedScopes = { "openid", "profile", "weather" }
                }
            };
    }
}