using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;

public class EventConsumer : IEventConsumer
{
    private readonly ConsumerConfig _config;
    private readonly IEventHandler<BaseEvent> _eventHandler;
    private readonly ILogger<EventConsumer> _logger;


    public EventConsumer(IOptions<ConsumerConfig> config, IEventHandler<BaseEvent> eventHandler,
        ILogger<EventConsumer> logger)
    {
        _eventHandler = eventHandler;
        _logger = logger;
        _config = config.Value;
    }

    public void RegisterEvents()
    {
        RegisterHandler(_eventHandler);
    }

    private void RegisterHandler<TEvent>(IEventHandler<TEvent> eventHandler) where TEvent : BaseEvent
    {
        eventHandler.On += EventHandler_On;
    }

    private void EventHandler_On<TEvent>(object? sender, TEvent e) where TEvent : BaseEvent
    {
        Console.WriteLine(e);
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
            var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
            var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, options);

            if (@event == null)
            {
                Console.WriteLine("Event not found");
                return;
            }

            _eventHandler.Handler(@event);


            consumer.Commit(consumerResult);
        }
    }
}