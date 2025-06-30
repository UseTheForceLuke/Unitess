using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TaskManager.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["IdentityServer:Authority"];
                options.Audience = configuration["IdentityServer:Audience"];
                options.RequireHttpsMetadata = bool.Parse(configuration["IdentityServer:RequireHttpsMetadata"]!);
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "role" && c.Value == "Admin")));

            options.AddPolicy("User", policy =>
                policy.RequireAuthenticatedUser());
        });

        return services;
    }
}