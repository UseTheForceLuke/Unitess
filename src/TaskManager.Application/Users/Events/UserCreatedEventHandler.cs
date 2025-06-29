using MediatR;
using TaskManager.Application.Abstraction;
using TaskManager.SharedKernel.Events;

namespace TaskManager.Application.Users.EventHandlers;

public class UserCreatedEventHandler : INotificationHandler<UserCreatedEvent>
{
    private readonly IUserSyncService _userSyncService;

    public UserCreatedEventHandler(IUserSyncService userSyncService)
    {
        _userSyncService = userSyncService;
    }

    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        await _userSyncService.GetOrCreateUserAsync(
            notification.SubjectId,
            notification.Username,
            notification.Email);
    }
}