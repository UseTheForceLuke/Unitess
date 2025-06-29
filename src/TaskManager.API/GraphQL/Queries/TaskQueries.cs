using Task = TaskManager.Domain.Tasks.Task;
using MediatR;
using TaskManager.Application.Tasks.Queries;

namespace TaskManager.API.GraphQL.Queries;

[ExtendObjectType(OperationTypeNames.Query)]
public class TaskQueries
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<Task>> GetTasks(
        [Service] IMediator mediator)
    {
        return await mediator.Send(new GetTasksQuery());
    }
}