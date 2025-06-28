using MediatR;
using TaskManager.Domain.Tasks.Repositories;
using TaskManager.Domain.Users.Repositories;
using TaskManager.Application.Abstraction;

namespace TaskManager.Application.Tasks.Commands;

public class CreateTaskCommandHandler : IRequestHandler<CreateTaskCommand, TaskDto>
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(ITaskRepository taskRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<TaskDto> Handle(CreateTaskCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

