using IdentityServer.Middleware;
using Microsoft.AspNetCore.Builder;

namespace IdentityServer.Extensions
{
    public static class GlobalExceptionHandlerExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandler>();
        }
    }
}
