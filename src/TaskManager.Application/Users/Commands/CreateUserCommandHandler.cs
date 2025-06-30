using MediatR;

namespace TaskManager.Application.Users.Commands;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // TODO: consume event from Identity server when user is registered

        throw new NotImplementedException();
    }
}
