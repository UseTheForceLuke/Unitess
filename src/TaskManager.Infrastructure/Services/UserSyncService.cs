using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskManager.Application.Abstraction;
using TaskManager.Domain.Users;

namespace TaskManager.Infrastructure.Services;

public class UserSyncService : IUserSyncService
{
    private readonly IApplicationDbContext _context;

    public UserSyncService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User> SyncUserFromClaimsAsync(ClaimsPrincipal principal)
    {
        var subjectId = principal.FindFirst("sub")?.Value;
        var username = principal.FindFirst("preferred_username")?.Value;
        var name = principal.FindFirst("name")?.Value;
        var email = principal.FindFirst("email")?.Value;

        if (string.IsNullOrEmpty(subjectId))
            throw new InvalidOperationException("No sub claim found");

        return await GetOrCreateUserAsync(subjectId, username, email);
    }

    public async Task<User> GetOrCreateUserAsync(string subjectId, string username, string email)
    {
        if (!Guid.TryParse(subjectId, out var userId))
            throw new ArgumentException("Invalid subject ID format");

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            user = new User
            {
                Id = userId,
                Username = username,
                Email = email,
                Role = UserRole.User // Default role
            };

            _context.Users.Attach(user);
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        return user;
    }

    public async Task<User> GetCurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        var currentUser = httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;

        if (currentUser == null)
            throw new UnauthorizedAccessException();

        return currentUser;
    }

}