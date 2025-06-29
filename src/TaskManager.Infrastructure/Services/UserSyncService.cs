using System.Security.Claims;
using TaskManager.Application.Abstraction;
using TaskManager.Domain.Users;
using TaskManager.Domain.Users.Repositories;

namespace TaskManager.Application.Services;

public class UserSyncService : IUserSyncService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserSyncService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<User> SyncUserFromClaimsAsync(ClaimsPrincipal principal)
    {
        var subjectId = principal.FindFirst("sub")?.Value;
        var username = principal.FindFirst("preferred_username")?.Value;
        var name = principal.FindFirst("name")?.Value;
        var email = principal.FindFirst("email")?.Value;

        if (string.IsNullOrEmpty(subjectId))
            throw new InvalidOperationException("No sub claim found");

        return await GetOrCreateUserAsync(subjectId, username, name, email);
    }

    public async Task<User> GetOrCreateUserAsync(string subjectId, string username, string name, string email)
    {
        if (!Guid.TryParse(subjectId, out var userId))
            throw new ArgumentException("Invalid subject ID format");

        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            user = new User
            {
                Id = userId,
                Username = username,
                Name = name,
                Email = email,
                Role = UserRole.User // Default role
            };

            await _userRepository.AddAsync(user);
            await _unitOfWork.CommitAsync(CancellationToken.None);
        }

        return user;
    }
}