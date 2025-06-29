using TaskManager.Application.Users.Commands;
using Task = TaskManager.Domain.Tasks.Task;

namespace TaskManager.Application.Tasks.Commands;

public class TaskDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public UserDto Creator { get; set; }
    public IEnumerable<UserDto> AssignedUsers { get; set; }

    public TaskDto(Task task)
    {
        Id = task.Id;
        Title = task.Title;
        Description = task.Description;
        Status = (TaskStatus)task.Status;
        CreatedAt = task.CreatedAt;
        //Creator = new UserDto(task.Creator);
        //AssignedUsers = task.UserTasks.Select(ut => new UserDto(ut.User));
    }
}