using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstraction;
using TaskManager.Application.Users.Commands;
using TaskManager.Domain;
using Task = TaskManager.Domain.Tasks.Task;

namespace TaskManager.Application.Tasks.Commands
{
    public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserSyncService _userSyncService;

        public CreateTaskCommandHandler(IApplicationDbContext context, IUserSyncService userSyncService)
        {
            _context = context;
            _userSyncService = userSyncService;
        }

        public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
        {
            var creator = await _userSyncService.GetCurrentUser();

            // 2. Validate assigned users exist
            var assignedUsers = await _context.Users
                .Where(u => request.AssignedUserIds.Contains(u.Id))
                .ToListAsync(cancellationToken);

            if (assignedUsers.Count != request.AssignedUserIds.Count())
                throw new Exception("One or more assigned users not found");

            // 3. Create the task
            var task = new Task(
                title: request.Title,
                description: request.Description,
                creatorId: creator.Id,
                status: request.Status
            );

            // 4. Add assigned users
            foreach (var userId in request.AssignedUserIds)
            {
                task.UserTasks.Add(new UserTask(userId, task.Id));
            }

            // 5. Save changes
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync(cancellationToken);

            // 6. Return the DTO
            return new TaskDto(task)
            {
                Creator = new UserDto(creator),
                AssignedUsers = assignedUsers.Select(u => new UserDto(u)).ToList()
            };
        }
    }
}