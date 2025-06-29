using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using TaskManager.Application.Abstraction;
using TaskManager.Infrastructure.Persistence;
using TaskManager.Infrastructure.Services;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("TaskManagerDb")));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<ApplicationDbContext>());

        services.AddHostedService<IdentityServerEventConsumer>();

        services.AddScoped<IUserSyncService, UserSyncService>();

        services.AddSingleton<IConnection>(sp => {
            var factory = new ConnectionFactory
            {
                HostName = configuration["EventBus:Host"],
                Port = int.Parse(configuration["EventBus:Port"]!),
                UserName = configuration["EventBus:Username"],
                Password = configuration["EventBus:Password"],
                VirtualHost = configuration["EventBus:VirtualHost"] ?? "/",
                //DispatchConsumersAsync = true  // Important for async consumers
            };
            return factory.CreateConnection();
        });

        return services;
    }
}