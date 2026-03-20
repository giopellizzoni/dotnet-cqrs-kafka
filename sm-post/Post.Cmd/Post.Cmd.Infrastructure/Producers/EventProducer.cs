using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Producers;
using Microsoft.Extensions.Options;

namespace Post.Cmd.Infrastructure.Producers;

public class EventProducer : IEventProducer, IDisposable
{
    private readonly IProducer<string, string> _producer;

    public EventProducer(IOptions<ProducerConfig> config)
    {
        _producer = new ProducerBuilder<string, string>(config.Value)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();
    }

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        var eventMessage = new Message<string, string>
        {
            Key = @event.Id.ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        var deliveryResult = await _producer.ProduceAsync(topic, eventMessage);

        if (deliveryResult.Status == PersistenceStatus.NotPersisted)
        {
            throw new MessageNotPersistedException(
                $"Could not produce {@event.GetType().Name} message topic - {topic} due to the following reason: {deliveryResult.Message}!");
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}