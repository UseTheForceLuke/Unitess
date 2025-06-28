using TaskManager.Domain.Users;
using TaskManager.SharedKernel;
using Task = TaskManager.Domain.Tasks.Task;

namespace TaskManager.Domain;

public class UserTask : Entity
{
    public int UserId { get; private set; }
    public User User { get; private set; }
    public int TaskId { get; private set; }
    public Task Task { get; private set; }
    public bool IsCreator { get; private set; }

    protected UserTask() { } // For EF

    public UserTask(int userId, int TaskId, bool isCreator = false)
    {
        UserId = userId;
        TaskId = TaskId;
        IsCreator = isCreator;
    }
}
