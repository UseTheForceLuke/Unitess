using System.Text;
using System.Text.Json;
using MediatR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TaskManager.SharedKernel.EventBus;
using TaskManager.SharedKernel.Events;

namespace TaskManager.Infrastructure;

public class IdentityServerEventConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly IMediator _mediator;
    private readonly ILogger<IdentityServerEventConsumer> _logger;

    public IdentityServerEventConsumer(
        IConnection connection,
        IMediator mediator,
        ILogger<IdentityServerEventConsumer> logger)
    {
        _connection = connection;
        _channel = _connection.CreateModel();
        _mediator = mediator;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _channel.ExchangeDeclare("identityserver.events", ExchangeType.Fanout);
        var queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queueName, "identityserver.events", "");

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var eventData = JsonSerializer.Deserialize<IdentityServerEvent>(message);

                if (eventData?.EventType == "UserCreated")
                {
                    var userEvent = new UserCreatedEvent(
                        eventData.SubjectId,
                        eventData.Username,
                        eventData.Name,
                        eventData.Email);

                    await _mediator.Publish(userEvent, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        };

        _channel.BasicConsume(queueName, true, consumer);
        return Task.CompletedTask;
    }
}