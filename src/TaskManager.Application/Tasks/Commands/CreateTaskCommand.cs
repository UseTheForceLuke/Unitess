using MediatR;
using TaskManager.Application.Tasks.Commands;
using TaskStatus = TaskManager.Domain.Tasks.TaskStatus;

namespace TaskManager.Application.Tasks;

public record CreateTaskCommand(
    string Title,
    string Description,
    TaskStatus Status,
    IEnumerable<Guid> AssignedUserIds
) : IRequest<TaskDto>;
