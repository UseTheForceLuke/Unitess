using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using IdentityServer;
using IdentityServer.Data;
using TaskManager.SharedKernel.EventBus;
using Microsoft.OpenApi.Models;
using Serilog;
using IdentityServer.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.Runtime.ConstrainedExecution;
using System.IO;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<AppIdentityDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("IdentityDb")));

// ASP.NET Core Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppIdentityDbContext>()
    .AddDefaultTokenProviders();

X509Certificate2 certificate = null;

if (builder.Environment.IsDevelopment())
{
    // For Docker development
    var certPath = "/root/.aspnet/https/IdentityServer.pfx";
    var certPassword = "your_password";/*builder.Configuration["CertificatePassword"];*/

    if (File.Exists(certPath))
    {
        certificate = new X509Certificate2(certPath, certPassword,
            X509KeyStorageFlags.MachineKeySet |
            X509KeyStorageFlags.PersistKeySet |
            X509KeyStorageFlags.Exportable);
    }
    else
    {
        // Fallback to memory-based keys if cert not found
        certificate = new X509Certificate2(
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ASP.NET", "Https", "IdentityServer.pfx"),
            builder.Configuration["CertificatePassword"]);
    }
}
else
{
    // Production configuration (Azure Key Vault, Kubernetes secrets, etc.)
    //certificate = /* Your production cert loading logic */;
}

builder.Services.AddIdentityServer(options =>
    {
        options.Events.RaiseErrorEvents = true;
        options.Events.RaiseInformationEvents = true;
        options.Events.RaiseFailureEvents = true;
        options.Events.RaiseSuccessEvents = true;
    })
    .AddAspNetIdentity<IdentityUser>()
    .AddSigningCredential(certificate)
    .AddDeveloperSigningCredential(persistKey: false, filename: null) // Persist key to file
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddInMemoryClients(Config.Clients);

// RabbitMQ Event Bus
builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory()
    {
        Uri = new Uri(builder.Configuration["EventBus:ConnectionString"])
    };
    return factory.CreateConnection();
});
builder.Services.AddScoped<IEventBus, RabbitMQEventBus>();

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddHealthChecks();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "IdentityServer API", Version = "v1" });
});

var app = builder.Build();

app.UseGlobalExceptionHandler();
app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseIdentityServer(); // Enables /.well-known/openid-configuration
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Add a default root endpoint
app.MapGet("/", () => Results.Redirect("/swagger"));

await InitializeAsync(builder, app);

app.Run();

static async System.Threading.Tasks.Task InitializeAsync(WebApplicationBuilder builder, WebApplication app)
{
    await DbInitializer.InitializeAsync(app.Services, builder.Configuration);

    if (app.Environment.IsDevelopment())
    {
        await DbInitializer.InitializeTestUsersAsync(app.Services, builder.Configuration);
    }
}