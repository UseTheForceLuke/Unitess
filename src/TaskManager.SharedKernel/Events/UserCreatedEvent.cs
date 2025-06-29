using MediatR;

namespace TaskManager.Application.Users.Events;

public record UserCreatedEvent(
    string SubjectId,
    string Username,
    string Email,
    string Role) : INotification;