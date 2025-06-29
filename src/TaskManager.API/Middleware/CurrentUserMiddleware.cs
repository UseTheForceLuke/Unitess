using TaskManager.Application.Abstraction;

namespace TaskManager.Api.Middleware;

public class CurrentUserMiddleware
{
    private readonly RequestDelegate _next;

    public CurrentUserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserSyncService userSyncService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                var user = await userSyncService.SyncUserFromClaimsAsync(context.User);
                context.Items["CurrentUser"] = user;
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request
                Console.WriteLine($"Error syncing user: {ex.Message}");
            }
        }

        await _next(context);
    }
}