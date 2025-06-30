using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Users.Commands;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstraction;

namespace TaskManager.API.GraphQL.Queries;

[ExtendObjectType("Query")]
public partial class Queries
{
    [Authorize(Policy = "AdminOrUser")]
    [UsePaging]
    [UseProjection]
    [UseFiltering(typeof(TaskFilterInputType))]
    [UseSorting]
    public IQueryable<TaskDto> GetTasks([Service] IApplicationDbContext context)
    {
        return context.Tasks
            .AsNoTracking()
            .Include(t => t.Creator)
            .Include(t => t.UserTasks)
                .ThenInclude(ut => ut.User)
            .Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                CreatedAt = t.CreatedAt,
                Creator = new UserDto { Id = t.Creator.Id, Username = t.Creator.Username },
                AssignedUsers = t.UserTasks
                    .Select(ut => new UserDto
                    {
                        Id = ut.User.Id,
                        Username = ut.User.Username
                    }).ToList()
            });
    }

    [Authorize(Policy = "AdminOrUser")]
    [GraphQLName("getTaskById")]
    public async Task<TaskDto> GetTaskById(
        [Service] IApplicationDbContext context,
        Guid id)
    {
        var task = await context.Tasks
            .Include(t => t.Creator)
            .FirstOrDefaultAsync(t => t.Id == id);

        return task == null
            ? throw new GraphQLException("Task not found")
            :new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                CreatedAt = task.CreatedAt,
                Creator = new UserDto { Id = task.Creator.Id, Username = task.Creator.Username },
            };
    }
}