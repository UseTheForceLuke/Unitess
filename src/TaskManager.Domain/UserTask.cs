using TaskManager.Domain.Users;
using TaskManager.SharedKernel;
using Task = TaskManager.Domain.Tasks.Task;

namespace TaskManager.Domain;

public class UserTask : Entity
{
    public Guid UserId { get; set; }
    public Guid TaskId { get; set; }
    public bool IsCreator { get; set; }

    // Navigation properties
    public User User { get; set; }
    public Task Task { get; set; }

    public UserTask(Guid userId, Guid taskId, bool isCreator = false)
    {
        UserId = userId;
        TaskId = taskId;
        IsCreator = isCreator;
    }
}
