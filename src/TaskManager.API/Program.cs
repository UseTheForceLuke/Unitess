using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using SharedKernel;
using TaskManager.Api.Middleware;
using TaskManager.API;
using TaskManager.API.GraphQL;
using TaskManager.API.GraphQL.Mutations;
using TaskManager.API.GraphQL.Queries;
using TaskManager.Application;
using TaskManager.Application.Abstraction;
using TaskManager.Application.Tasks;
using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Users.Commands;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
    .AddEnvironmentVariables();

builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication()
    .AddPresentation(builder.Configuration);

builder.Services
    .AddGraphQLServer()
    .AddSorting(config =>
    {
        config.AddDefaults();
    })
    .ModifyOptions(options =>
    {
        options.DefaultBindingBehavior = BindingBehavior.Explicit; // Only register explicit types
    })
    .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<Queries>()
    .AddMutationType(d => d.Name("Mutation"))
        .AddTypeExtension<Mutations>()
    // Register all types explicitly
    .AddType<TaskStatusType>()
    .AddType<CreateTaskInputType>()
    .AddType<UserInputType>()
    .AddType<TaskSortType>()
    .AddType<TaskDtoType>()
    .AddType<UserDtoType>()
    .AddType<TaskDtoSortType>()
    .AddFiltering()
    .AddProjections()
    .AddErrorFilter(error =>
    {
        if (error.Exception is UnauthorizedAccessException)
            return error.WithMessage("Unauthorized access");
        return error;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UsePlayground(new PlaygroundOptions
    {
        Path = "/playground",
        QueryPath = "/graphql",
        SubscriptionPath = "/graphql",
    });
}

app.UseRouting();

// Add this middleware before UseAuthorization()
app.Use(async (context, next) =>
{
    // Check if request is coming from Playground
    if (context.Request.Path.StartsWithSegments("/graphql") &&
        context.Request.Headers.TryGetValue("Referer", out var referer) &&
        referer.ToString().Contains("playground"))
    {
        // Skip auth for playground
        await next();
    }
    else
    {
        // Apply auth for all other requests
        await next();
    }
});
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL()
    .RequireAuthorization();
});

ApplyMigrations(app);

app.Run();

static void ApplyMigrations(WebApplication app) 
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        dbContext.Database.Migrate();
    }
}
