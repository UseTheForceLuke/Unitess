using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.Tasks;

namespace TaskManager.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(CreateTaskCommand).Assembly));

        services.AddValidatorsFromAssemblyContaining<CreateTaskCommandValidator>();

        return services;
    }
}