using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace IdentityServer;

public static class Config
{
    public static IEnumerable<ApiScope> ApiScopes => new[]
    {
        new ApiScope("taskmanager.api", "Task Manager API")
    };

    public static IEnumerable<Client> Clients => new[]
    {
        new Client
        {
            ClientId = "taskmanager.client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { "taskmanager.api", "openid", "profile" }
        }
    };
}
