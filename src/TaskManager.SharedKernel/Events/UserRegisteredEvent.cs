using MediatR;

namespace TaskManager.SharedKernel.Events;

public record UserRegisteredEvent(
    string IdentityId,
    string Username,
    string Email,
    string Role) : INotification;
