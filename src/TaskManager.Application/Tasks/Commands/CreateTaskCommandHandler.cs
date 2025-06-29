using MediatR;
using TaskManager.Application.Abstraction;
using TaskManager.Domain;
using Task = TaskManager.Domain.Tasks.Task;

namespace TaskManager.Application.Tasks.Commands;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly IApplicationDbContext _context;

    public CreateTaskCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskDto> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var creator = await _context.Users
            .FindAsync(new object[] { request.CreatorId }, cancellationToken);

        if (creator == null)
            throw new InvalidOperationException("Creator not found");

        var task = new Task(request.Title, request.Description, request.CreatorId);

        await _context.Tasks.AddAsync(task, cancellationToken);

        if (request.AssignedUserIds.Any())
        {
            foreach (var userId in request.AssignedUserIds)
            {
                var user = await _context.Users
                    .FindAsync(new object[] { userId }, cancellationToken);

                if (user != null)
                {
                    task.UserTasks.Add(new UserTask(userId, task.Id));
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return new TaskDto(task);
    }
}