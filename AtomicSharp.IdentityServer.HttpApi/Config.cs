using System.Collections.Generic;
using IdentityServer4.Models;

namespace AtomicSharp.IdentityServer.HttpApi
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
                new ApiResource("ids-admin", "Administration of IdentityServer")
                {
                    Scopes = {"weather"}
                }
            };


        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    ClientId = "unified-admin",
                    ClientName = "AtomicSharp.Admin.Web",
                    ClientSecrets = {new Secret("admin".Sha256())},

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = true,

                    RedirectUris = {"https://www.atomicsharp.dev/admin/oauth/signin-oidc"},
                    PostLogoutRedirectUris = {"https://www.atomicsharp.dev/admin/oauth/signout-oidc"},

                    AllowOfflineAccess = true,
                    AllowedScopes = {"openid", "profile", "weather"}
                }
            };
    }
}