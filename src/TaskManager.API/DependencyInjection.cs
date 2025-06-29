using TaskManager.Api.Middleware;
namespace TaskManager.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddTransient<CurrentUserMiddleware>();

        return services;
    }
}