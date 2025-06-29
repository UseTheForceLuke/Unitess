using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using TaskManager.Api.Middleware;
using TaskManager.API;
using TaskManager.Application;
using TaskManager.Application.Abstraction;
using TaskManager.Application.Services;
using TaskManager.Application.Tasks;
using TaskManager.Infrastructure;

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

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                c.Type == "role" && c.Value == "Admin")));

    options.AddPolicy("User", policy =>
        policy.RequireAuthenticatedUser());
});
app.UseMiddleware<CurrentUserMiddleware>();

app.MapControllers();

app.Run();
