using TaskManager.Application.Users.Commands;

namespace TaskManager.API.Mutations;

public class CreateTaskInput
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserDto Creator { get; set; }
    public IEnumerable<Guid> AssignedUserIds { get; set; }
}