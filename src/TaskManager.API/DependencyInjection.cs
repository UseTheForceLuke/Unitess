using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace TaskManager.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO: Remove all hardscoded code

        services.AddHttpContextAccessor();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "http://localhost:5011";
                options.Audience = "taskmanager.api";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", // "role", used to be This is critical
                    ValidateIssuer = true,
                    ValidIssuer = "http://localhost:5011",
                    ValidateAudience = true,
                    ValidAudience = "taskmanager.api"
                };
                options.RequireHttpsMetadata = false;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
            options.AddPolicy("User", policy =>
                policy.RequireClaim(
                    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                    "User"));
        });

        return services;
    }
}