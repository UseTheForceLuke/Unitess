using Microsoft.EntityFrameworkCore;
using TaskManager.Domain;
using TaskManager.Domain.Users;
using Task = TaskManager.Domain.Tasks.Task;

namespace TaskManager.Application.Abstraction;

public interface IApplicationDbContext
{
    DbSet<Task> Tasks { get; }
    DbSet<User> Users { get; }
    DbSet<UserTask> UserTasks { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}