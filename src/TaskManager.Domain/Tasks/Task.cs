using TaskManager.Domain.Users;
using TaskManager.SharedKernel;

namespace TaskManager.Domain.Tasks;

public class Task : Entity
{
    public Guid Id { get; set; }
    public string Title { get; private set; }
    public string Description { get; private set; }
    public TaskStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatorId { get; private set; }

    // Navigation properties
    public User Creator { get; private set; }
    public ICollection<UserTask> UserTasks { get; private set; } = new List<UserTask>();

    protected Task() { } // For EF

    public Task(string title, string description, Guid creatorId)
    {
        Title = title;
        Description = description;
        Status = TaskStatus.New;
        CreatedAt = DateTime.UtcNow;
        CreatorId = creatorId;
    }

    public Task(string title, string description, Guid creatorId, TaskStatus status)
    {
        Title = title;
        Description = description;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        CreatorId = creatorId;
    }

    public void Update(string title, string description, TaskStatus status)
    {
        Title = title;
        Description = description;
        Status = status;
    }
}
