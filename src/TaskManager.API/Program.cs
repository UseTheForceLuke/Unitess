using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Playground;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TaskManager.Api.Middleware;
using TaskManager.API;
using TaskManager.API.GraphQL;
using TaskManager.API.GraphQL.Mutations;
using TaskManager.API.GraphQL.Queries;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.Persistence;
using System.Diagnostics;

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
    .AddAuthorization() // Enables @authorize directive
    .ModifyRequestOptions(options =>
    {
        options.IncludeExceptionDetails = true; // Show exception details
    })
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
        // Custom error handling
        if (error.Exception is not null)
        {
            Debug.WriteLine($"GraphQL Error: {error.Exception}", ConsoleColor.Red);
        }
        return error;
    });

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.Authority = "http://localhost:5011";
//        options.Audience = "taskmanager.api";

//        // Configure token validation
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateAudience = true,
//            ValidAudience = "taskmanager.api",
//            ValidateIssuer = true,
//            ValidIssuer = "http://localhost:5011",
//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.Zero,
//            RoleClaimType = "role",
//            NameClaimType = "name",
//            RequireSignedTokens = true,
//            ValidateIssuerSigningKey = true,

//            // Simplified key resolver
//            IssuerSigningKeyResolver = async (token, securityToken, kid, parameters) =>
//            {
//                using var client = new HttpClient();
//                var discoveryDocument = await client.GetDiscoveryDocumentAsync("http://localhost:5011");

//                if (discoveryDocument.IsError)
//                {
//                    throw new Exception($"Discovery error: {discoveryDocument.Error}");
//                }

//                return discoveryDocument.KeySet.Keys;
//            }
//        };

//        options.RequireHttpsMetadata = false;
//    });
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "http://localhost:5011";
        options.Audience = "taskmanager.api";

        // Add these two critical lines:
        options.RequireHttpsMetadata = false; // Disable HTTPS requirement
        options.BackchannelHttpHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidAudience = "taskmanager.api",
            ValidateIssuer = true,
            ValidIssuer = "http://localhost:5011",
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "role",
            NameClaimType = "name"
        };

        // Add this exact configuration:
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
            {
                var client = new HttpClient();
                var jwks = client.GetStringAsync($"{options.Authority}/.well-known/openid-configuration/jwks").Result;
                return new JsonWebKeySet(jwks).GetSigningKeys();
            },
            // Rest of your validation parameters...
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("User", policy =>
        policy.RequireAuthenticatedUser()
              .RequireClaim("role", "User")); // Changed from RequireRole to RequireClaim
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

app.UseAuthentication();
app.UseAuthorization();

//app.UseMiddleware<CurrentUserMiddleware>();

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
