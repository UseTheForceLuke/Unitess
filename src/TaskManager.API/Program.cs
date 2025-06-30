using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TaskManager.API;
using TaskManager.API.GraphQL;
using TaskManager.API.GraphQL.Mutations;
using TaskManager.API.GraphQL.Queries;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Persistence;
using System.Diagnostics;
using TaskManager.Api.Middleware;

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
    .AddAuthorization() // Enables @authorize directives
    .ModifyOptions(options =>
    {
        options.DefaultBindingBehavior = BindingBehavior.Explicit; // Only register explicit types
    })
    .ModifyRequestOptions(options =>
    {
        options.IncludeExceptionDetails = true; // Show exception details
    })
    .AddSorting(config =>
    {
        config.AddDefaults();
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
        if (error.Exception is not null)
        {
            Debug.WriteLine($"GraphQL Error: {error.Exception}", ConsoleColor.Red);
        }
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

// For Debug token
//app.Use(async (context, next) =>
//{
//    // Log incoming token
//    var token = context.Request.Headers["Authorization"];
//    Console.WriteLine($"Token: {token}");

//    await next();

//    // Log authenticated user claims
//    if (context.User.Identity?.IsAuthenticated == true)
//    {
//        Console.WriteLine("User claims:");
//        foreach (var claim in context.User.Claims)
//        {
//            Console.WriteLine($"{claim.Type} = {claim.Value}");
//        }
//    }
//});

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<CurrentUserMiddleware>();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
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
