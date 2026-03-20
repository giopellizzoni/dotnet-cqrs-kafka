using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Post.Query.Infrastructure.Consumers;

public class ConsumerHostedServices : IHostedService
{
    private readonly ILogger<ConsumerHostedServices> _logger;
    private readonly IServiceProvider _serviceProvider;

    public ConsumerHostedServices(ILogger<ConsumerHostedServices> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event consumer service running.");
        var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC")
            ?? throw new InvalidOperationException("KAFKA_TOPIC environment variable is not set.");

        using var scope = _serviceProvider.CreateScope();
        var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();

        await Task.Run(() => eventConsumer.Consume(topic, cancellationToken), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event consumer service stopped.");
        return Task.CompletedTask;
    }
}