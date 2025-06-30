using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using TaskManager.SharedKernel;

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
                options.Authority = "http://localhost:5011"; // TODO: un-hardode and put in env
                options.Audience = "taskmanager.api";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = Claims.Role,
                    ValidateIssuer = true,
                    ValidIssuer = "http://localhost:5011",
                    ValidateAudience = true,
                    ValidAudience = "taskmanager.api"
                };
                options.RequireHttpsMetadata = false;
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
                policy.RequireClaim(
                    Claims.Role, "Admin"));
            options.AddPolicy("User", policy =>
                policy.RequireClaim(
                    Claims.Role,
                    "User"));
            // Combined OR policy
            options.AddPolicy("AdminOrUser", policy =>
                policy.RequireAssertion(context =>
                    context.User.IsInRole("Admin") ||
                    context.User.IsInRole("User")
                )
            );
        });

        return services;
    }
}