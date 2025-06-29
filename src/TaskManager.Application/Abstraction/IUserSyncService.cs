using System.Security.Claims;
using TaskManager.Domain.Users;

namespace TaskManager.Application.Abstraction;

public interface IUserSyncService
{
    Task<User> SyncUserFromClaimsAsync(ClaimsPrincipal principal);
    Task<User> GetOrCreateUserAsync(string subjectId, string username, string name, string email);
}