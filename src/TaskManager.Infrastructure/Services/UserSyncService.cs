﻿using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TaskManager.Application.Abstraction;
using TaskManager.Domain.Users;
using TaskManager.SharedKernel;

namespace TaskManager.Infrastructure.Services;

public class UserSyncService : IUserSyncService
{
    private readonly IApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserSyncService(IApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User> SyncUserFromClaimsAsync(ClaimsPrincipal principal)
    {
        var subjectId = principal.FindFirst(Claims.Sub)?.Value;
        var role = principal.FindFirst(Claims.Role)?.Value;

        // TODO: get profile info req
        //var username = principal.FindFirst("preferred_username")?.Value;
        //var name = principal.FindFirst("name")?.Value;
        //var email = principal.FindFirst("email")?.Value;

        if (string.IsNullOrEmpty(subjectId))
            throw new InvalidOperationException("No sub claim found");

        return await GetOrCreateUserAsync(subjectId, string.Empty, string.Empty, role);
    }

    public async Task<User> GetOrCreateUserAsync(string subjectId, string username, string email, string role)
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
                Role = Enum.Parse<UserRole>(role)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(CancellationToken.None);
        }

        return user;
    }

    public async Task<User> GetCurrentUser()
    {
        var currentUser = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;

        if (currentUser == null)
            throw new UnauthorizedAccessException();

        return currentUser;
    }

}