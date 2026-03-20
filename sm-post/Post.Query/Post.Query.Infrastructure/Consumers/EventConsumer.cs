using System.Text.Json;
using System.Text.Json.Nodes;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Post.Common.Events;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _config;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventConsumer> _logger;
    private readonly Dictionary<string, Func<Message<string, string>, Task>> _eventHandlers;

    public EventConsumer(IOptions<ConsumerConfig> config, IServiceProvider serviceProvider,
        ILogger<EventConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _config = config.Value;
        _eventHandlers = new Dictionary<string, Func<Message<string, string>, Task>>
        {
            { nameof(PostCreatedEvent), HandleEvent<PostCreatedEvent> },
            { nameof(PostLikedEvent), HandleEvent<PostLikedEvent> },
            { nameof(MessageUpdatedEvent), HandleEvent<MessageUpdatedEvent> },
            { nameof(PostRemovedEvent), HandleEvent<PostRemovedEvent> },
            { nameof(CommentAddedEvent), HandleEvent<CommentAddedEvent> },
            { nameof(CommentUpdatedEvent), HandleEvent<CommentUpdatedEvent> },
            { nameof(CommentRemovedEvent), HandleEvent<CommentRemovedEvent> },
        };
    }

    public void Consume(string topic, CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);
        while (!cancellationToken.IsCancellationRequested)
        {
            var consumerResult = consumer.Consume(TimeSpan.FromSeconds(1));
            if (consumerResult?.Message == null) continue;

            _logger.LogInformation("Consuming message: {Value}", consumerResult.Message.Value);

            var eventType = JsonNode.Parse(consumerResult.Message.Value)?["Type"]?.ToString();
            if (eventType == null)
            {
                _logger.LogWarning("Could not determine event type from message. Skipping.");
                continue;
            }

            if (!_eventHandlers.TryGetValue(eventType, out var handler))
            {
                _logger.LogWarning("No handler registered for event type: {EventType}. Skipping.", eventType);
                continue;
            }

            try
            {
                handler(consumerResult.Message).GetAwaiter().GetResult();
                consumer.Commit(consumerResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to handle event of type {EventType}. Offset not committed.", eventType);
                throw;
            }
        }
    }

    private async Task HandleEvent<T>(Message<string, string> message) where T : BaseEvent
    {
        var eventHandler = _serviceProvider.GetRequiredService<IEventHandler<T>>();
        var @event = JsonSerializer.Deserialize<T>(message.Value);
        await eventHandler.Handler(@event);
    }
}