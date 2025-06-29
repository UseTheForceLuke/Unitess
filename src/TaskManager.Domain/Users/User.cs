using Task = TaskManager.Domain.Tasks.Task;

namespace TaskManager.Domain.Users;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public UserRole Role { get; set; }
    public ICollection<Task> CreatedTasks { get; set; } = new List<Task>();
    public ICollection<UserTask> AssignedTasks { get; set; } = new List<UserTask>();
}
