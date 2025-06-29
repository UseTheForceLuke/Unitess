using Task = TaskManager.Domain.Tasks.Task;

namespace TaskManager.Domain.Users;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }

    // One-to-Many: Tasks created by this user
    public ICollection<Task> CreatedTasks { get; set; } = new List<Task>();

    // Many-to-Many: Tasks assigned to this user
    public ICollection<UserTask> UserTasks { get; set; } = new List<UserTask>();
}