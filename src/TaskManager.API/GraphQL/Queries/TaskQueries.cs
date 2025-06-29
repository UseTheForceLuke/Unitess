using Task = TaskManager.Domain.Tasks.Task;
using TaskManager.Infrastructure.Persistence;

namespace TaskManager.API.GraphQL.Queries;

[ExtendObjectType("Query")]
public class TaskQueries
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Task> GetTasks(
        [Service] ApplicationDbContext context)
    {
        return context.Tasks;
    }

}