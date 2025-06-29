using HotChocolate.Authorization;
using HotChocolate.Types;
using MediatR;
using TaskManager.API.Mutations;
using TaskManager.Application.Tasks;
using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Users.Commands;
using TaskManager.Domain.Users;

namespace TaskManager.API.GraphQL.Mutations;

[ExtendObjectType(OperationTypeNames.Mutation)]
public class TaskMutations
{
    //[Authorize(Policy = "User")]
    public async Task<TaskDto> CreateTask(
        CreateTaskInput input,
        [Service] IMediator mediator,
        [Service] IHttpContextAccessor httpContextAccessor)
    {
        var currentUser = httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
        if (currentUser == null)
            throw new UnauthorizedAccessException();

        var command = new CreateTaskCommand(
            input.Title,
            input.Description,
            currentUser.Id,
            input.AssignedUserIds);

        return await mediator.Send(command);
    }

    //[Authorize(Policy = "User")]
    //public async Task<TaskDto> UpdateTask(
    //    UpdateTaskInput input,
    //    [Service] IMediator mediator,
    //    [Service] IHttpContextAccessor httpContextAccessor)
    //{
    //    var currentUser = httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
    //    if (currentUser == null)
    //        throw new UnauthorizedAccessException();

    //    var command = new UpdateTaskCommand(
    //        input.Id,
    //        input.Title,
    //        input.Description,
    //        input.Status,
    //        currentUser.Id);

    //    return await mediator.Send(command);
    //}

    //[Authorize(Policy = "Admin")]
    //public async Task<bool> DeleteTask(
    //    Guid id,
    //    [Service] IMediator mediator)
    //{
    //    var command = new DeleteTaskCommand(id);
    //    await mediator.Send(command);
    //    return true;
    //}

    //[Authorize(Policy = "Admin")]
    //public async Task<bool> AssignTaskToUser(
    //    Guid taskId,
    //    Guid userId,
    //    [Service] IMediator mediator)
    //{
    //    var command = new AssignTaskCommand(taskId, userId);
    //    await mediator.Send(command);
    //    return true;
    //}
}