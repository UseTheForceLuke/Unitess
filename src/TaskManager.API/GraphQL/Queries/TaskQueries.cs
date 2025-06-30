using TaskManager.Infrastructure.Persistence;
using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Users.Commands;
using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.API.GraphQL.Queries;

[ExtendObjectType("Query")]
public partial class Queries
{
    [Authorize(Policy = "AdminOrUser")]
    [UsePaging]
    [UseProjection]
    [UseFiltering(typeof(TaskFilterInputType))]
    [UseSorting]
    public IQueryable<TaskDto> GetTasks([Service] ApplicationDbContext context)
    {
        return context.Tasks
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
}