using MediatR;
using TaskManager.Application.Tasks.Commands;

namespace TaskManager.Application.Tasks;

public record CreateTaskCommand(
    string Title, 
    string Description,
    Guid CreatorId, 
    IEnumerable<Guid> AssignedUserIds) : IRequest<TaskDto>;
