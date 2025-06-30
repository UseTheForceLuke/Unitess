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
            ClientId = "taskmanager.api", // TODO: do not hardoce it move to appsettings
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret("secret".Sha256()) }, // TODO: do not hardoce it move to appsettings
            AllowedScopes = { "taskmanager.api", "openid", "profile" }, // TODO: do not hardoce it move to appsettings
            AlwaysIncludeUserClaimsInIdToken = true, // TODO: do not hardoce it move to appsettings
            AlwaysSendClientClaims = true, // TODO: do not hardoce it move to appsettings,
            Claims = new[]
            {
                new ClientClaim("iss", "http://localhost:5011") // Match Authority
            }
        }
    };
}
