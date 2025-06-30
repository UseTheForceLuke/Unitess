using MediatR;
using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Tasks;
using HotChocolate.Authorization;

namespace TaskManager.API.GraphQL.Mutations;

[ExtendObjectType("Mutation")]
public partial class Mutations
{
    [Authorize(Policy = "AdminOrUser")]
    public async Task<TaskDto> CreateTask(
        [Service] IMediator mediator,
        CreateTaskInput input,
        CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateTaskCommand(
            Title: input.Title,
            Description: input.Description,
            Status: input.Status,
            AssignedUserIds: input.AssignedUserIds.Select(Guid.Parse) // TODO: add validator
        ), cancellationToken);
    }
}