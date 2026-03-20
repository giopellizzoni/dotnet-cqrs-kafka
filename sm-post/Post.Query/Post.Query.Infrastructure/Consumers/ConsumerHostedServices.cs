using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Post.Query.Infrastructure.Consumers;

public class ConsumerHostedServices : IHostedService
{
    private readonly ILogger<ConsumerHostedServices> _logger;
    private readonly IServiceProvider _serviceProvider;
    private Task? _consumerTask;

    public ConsumerHostedServices(
        ILogger<ConsumerHostedServices> logger,
        IServiceProvider serviceProvider
    )
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event consumer service running.");
        var topic =
            Environment.GetEnvironmentVariable("KAFKA_TOPIC")
            ?? throw new InvalidOperationException("KAFKA_TOPIC environment variable is not set.");

        _consumerTask = Task.Run(
            () =>
            {
                using var scope = _serviceProvider.CreateScope();
                var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                eventConsumer.Consume(topic, cancellationToken);
            },
            cancellationToken
        );

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event consumer service stopped.");
        if (_consumerTask != null)
            await _consumerTask.WaitAsync(cancellationToken).ConfigureAwait(false);
    }
}
