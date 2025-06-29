using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstraction;
using TaskManager.Domain;
using TaskManager.Domain.Users;
using Task = TaskManager.Domain.Tasks.Task;
using Task_ = System.Threading.Tasks.Task;

namespace TaskManager.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
{
    public DbSet<Task> Tasks { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserTask> UserTasks { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public async Task_ CommitAsync(CancellationToken cancellationToken)
    {
        await SaveChangesAsync(cancellationToken);
    }
}