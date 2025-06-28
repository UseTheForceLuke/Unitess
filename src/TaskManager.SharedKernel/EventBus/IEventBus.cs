using MediatR;

namespace TaskManager.SharedKernel.EventBus;

public interface IEventBus
{
    Task Publish<T>(T @event) where T : INotification;
}
