using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<ApiScope> ApiScopes => new[]
    {
        new ApiScope("taskmanager.api", "Task Manager API")
    };

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),     // standard openid (required)
            new IdentityResources.Profile()    // profile claims (name, etc.)
        };

    public static IEnumerable<Client> Clients => new[]
    {
        new Client
        {
            ClientId = "taskmanager.client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret("secret".Sha256()) }, // TODO: do not hardoce it
            AllowedScopes = { "taskmanager.api", "openid", "profile" },
            AlwaysIncludeUserClaimsInIdToken = true,
            AlwaysSendClientClaims = true
        }
    };
}
