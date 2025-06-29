using MediatR;

namespace TaskManager.SharedKernel.Events;

public record UserCreatedEvent(
    string SubjectId,
    string Username,
    string Email,
    string Role) : INotification;