using MediatR;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;

namespace TaskManager.SharedKernel.EventBus;

public class RabbitMQEventBus : IEventBus
{
    private readonly IConnection _connection;

    public RabbitMQEventBus(IConnection connection)
    {
        _connection = connection;
    }

    public async Task Publish<T>(T @event) where T : INotification
    {
        using var channel = _connection.CreateModel();
        var exchangeName = typeof(T).Name;

        channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout);

        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        channel.BasicPublish(exchangeName, "", null, body);
    }
}