using TaskStatus = TaskManager.Domain.Tasks.TaskStatus;

namespace TaskManager.API.GraphQL.Mutations;

public class CreateTaskInput
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }
    public IEnumerable<string> AssignedUserIds { get; set; } = new List<string>();
}