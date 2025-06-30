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
    .AddPresentation();

// Add authentication (make sure to configure your JWT bearer options)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5012";
        options.Audience = "taskmanager.api";
        options.RequireHttpsMetadata = false; // Only for development
    });

builder.Services
    .AddGraphQLServer()
    .AddSorting(config =>
    {
        config.AddDefaults();
        //config.BindRuntimeType<TaskManager.Domain.Tasks.Task, TaskSortType>();
    })
    .ModifyOptions(options =>
    {
        options.DefaultBindingBehavior = BindingBehavior.Explicit; // Only register explicit types
    })
    .AddQueryType(d => d.Name("Query"))
        .AddTypeExtension<TaskQueries>()
    // Register all types explicitly
    .AddType<TaskStatusType>()
    .AddType<CreateTaskInputType>()
    .AddType<UserInputType>()
    .AddType<TaskSortType>()
    .AddType<TaskDtoType>() // Add this
    .AddType<UserDtoType>() // Add this
    .AddType<TaskDtoSortType>() // Uncomment this
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
        SubscriptionPath = "/graphql"
    });
}

app.UseRouting();
app.UseEndpoints(endpoints =>
{
    endpoints.MapGraphQL();
});

ApplyMigrations(app);

//app.UseAuthentication();
//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("Admin", policy =>
//        policy.RequireAssertion(context =>
//            context.User.HasClaim(c =>
//                c.Type == "role" && c.Value == "Admin")));

//    options.AddPolicy("User", policy =>
//        policy.RequireAuthenticatedUser());
//});
app.UseMiddleware<CurrentUserMiddleware>();

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
