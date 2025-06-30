using Duende.IdentityModel;
using Duende.IdentityServer.Models;
using System.Collections.Generic;

namespace IdentityServer;

public static class Config
{
    // In Config.cs
    public static IEnumerable<ApiResource> ApiResources => new[]
    {
        new ApiResource("taskmanager.api", "Task Manager API") // This becomes the audience
        {
            Scopes = { "taskmanager.api" },
            UserClaims = { JwtClaimTypes.Role }
        }
    };

    public static IEnumerable<ApiScope> ApiScopes => new[]
    {
        new ApiScope("taskmanager.api", "Task Manager API")
    };

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
            new IdentityResource("roles", new[] { JwtClaimTypes.Role }) // Add roles identity resource
        };

    public static IEnumerable<Client> Clients => new[]
    {
        new Client
        {

            ClientId = "taskmanager.api",  // Using your required client ID
            ClientName = "Task Manager API Client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
            ClientSecrets = { new Secret("secret".Sha256()) },
            AllowedScopes = { "taskmanager.api", "openid", "profile", "roles" },
            AccessTokenType = AccessTokenType.Jwt,
            AlwaysIncludeUserClaimsInIdToken = true,
            UpdateAccessTokenClaimsOnRefresh = true,
        
            // Set access token lifetime (in seconds)
            AccessTokenLifetime = 3600 * 24 * 7, // 1 week (adjust as needed)

            // These ensure the audience claim is included
            ClientClaimsPrefix = "",
            Claims = new[]
            {
                new ClientClaim("aud", "taskmanager.api"),  // Explicit audience claim
                new ClientClaim("role", "User") // Ensure claim type is "role"
            }
        }
    };
}
