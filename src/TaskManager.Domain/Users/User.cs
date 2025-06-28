using Task = TaskManager.Domain.Tasks.Task;

namespace TaskManager.Domain.Users;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public ICollection<Task> CreatedTasks { get; set; }
    public ICollection<Task> AssignedTasks { get; set; }
}
