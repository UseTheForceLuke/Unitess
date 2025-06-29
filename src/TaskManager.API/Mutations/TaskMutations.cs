using TaskManager.Application.Abstraction;
using TaskManager.Application.Tasks.Commands;
using TaskManager.Application.Tasks;
using TaskManager.Domain.Tasks.Repositories;
using TaskManager.Domain.Users.Repositories;
using TaskManager.Domain.Users;

namespace TaskManager.API.Mutations;

public class TaskMutations
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TaskMutations(
        ITaskRepository taskRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        IHttpContextAccessor httpContextAccessor)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TaskDto> CreateTask(CreateTaskInput input)
    {
        var currentUser = _httpContextAccessor.HttpContext?.Items["CurrentUser"] as User;
        if (currentUser == null)
            throw new UnauthorizedAccessException();

        var command = new CreateTaskCommand(
            input.Title,
            input.Description,
            currentUser.Id,
            input.AssignedUserIds);

        var handler = new CreateTaskCommandHandler(_taskRepository, _userRepository, _unitOfWork);
        return await handler.Handle(command, CancellationToken.None);
    }
}