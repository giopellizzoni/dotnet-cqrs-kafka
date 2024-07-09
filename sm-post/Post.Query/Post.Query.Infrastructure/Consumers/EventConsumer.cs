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


    public EventConsumer(IOptions<ConsumerConfig> config, IServiceProvider serviceProvider,
        ILogger<EventConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _config = config.Value;
    }

    public void Consume(string topic)
    {
        using var consumer = new ConsumerBuilder<string, string>(_config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(Deserializers.Utf8)
            .Build();

        consumer.Subscribe(topic);
        while (true)
        {
            var consumerResult = consumer.Consume();
            if (consumerResult?.Message == null) continue;

            _logger.LogInformation("Consuming message: {0}", consumerResult.Message.Value);
            
            var eventType = JsonNode.Parse(consumerResult.Message.Value)?["Type"];
            if (eventType == null)
            {
                Console.WriteLine("Event not found");
                return;
            }

            var eventHandlers = new Dictionary<string, Action<Message<string, string>>>
            {
                { nameof(PostCreatedEvent), HandleEvent<PostCreatedEvent> },
                { nameof(PostLikedEvent), HandleEvent<PostLikedEvent> },
                { nameof(MessageUpdatedEvent), HandleEvent<MessageUpdatedEvent> },
                { nameof(PostRemovedEvent), HandleEvent<PostRemovedEvent> },
                { nameof(CommentAddedEvent), HandleEvent<CommentAddedEvent> },
                { nameof(CommentUpdatedEvent), HandleEvent<CommentUpdatedEvent> },
                { nameof(CommentRemovedEvent), HandleEvent<CommentRemovedEvent> },
            };

            eventHandlers[eventType.ToString()](consumerResult.Message);
            consumer.Commit(consumerResult);
        }
    }

    private void HandleEvent<T>(Message<string, string> message) where T : BaseEvent
    {
        var eventHandler = _serviceProvider.GetRequiredService<IEventHandler<T>>();
        var @event = JsonSerializer.Deserialize<T>(message.Value);
        eventHandler.Handler(@event);
    }
}