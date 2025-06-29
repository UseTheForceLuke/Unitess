using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using TaskManager.Api.Middleware;
using TaskManager.API;
using TaskManager.API.GraphQL.Mutations;
using TaskManager.API.GraphQL.Queries;
using TaskManager.Application;
using TaskManager.Application.Abstraction;
using TaskManager.Application.Tasks;
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
    .AddQueryType<TaskQueries>()
    .AddMutationType<TaskMutations>()
    .AddProjections()
    .AddFiltering()
    .AddSorting()
    .AddErrorFilter(error =>
    {
        if (error.Exception is UnauthorizedAccessException)
            return error.WithMessage("Unauthorized access");
        return error;
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UsePlayground();
}

//app.UseHttpsRedirection();

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
