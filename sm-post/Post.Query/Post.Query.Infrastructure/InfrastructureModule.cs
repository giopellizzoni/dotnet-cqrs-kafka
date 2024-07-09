using System.Reflection;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Post.Common.Events;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.DataAcces;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        IConfigurationManager configuration)
    {
        services
            .AddDbContext(configuration)
            .AddConsumer(configuration)
            .AddEventHandlers();
        services.AddHostedService<ConsumerHostedServices>();

        return services;
    }


    private static IServiceCollection AddDbContext(this IServiceCollection services,
        IConfigurationManager configuration)
    {
        Action<DbContextOptionsBuilder> configureDbContext =
            (o => o.UseLazyLoadingProxies().UseSqlServer(configuration.GetConnectionString("SqlServer")));
        services.AddDbContext<DatabaseContext>(configureDbContext);
        services.AddSingleton(new DatabaseContextFactory(configureDbContext));

        //Create database and tables
        var dataContext = services.BuildServiceProvider().GetRequiredService<DatabaseContext>();
        dataContext.Database.EnsureCreated();

        return services;
    }

    private static IServiceCollection AddConsumer(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.Configure<ConsumerConfig>(configuration.GetSection(nameof(ConsumerConfig)));
        return services;
    }

    private static IServiceCollection AddEventHandlers(this IServiceCollection services)
    {
        services.AddScoped(typeof(IEventHandler<>));
        
        // services.AddScoped<IEventHandler<PostCreatedEvent>, PostCreatedEventHandler>();
        // services.AddScoped<IEventHandler<PostLikedEvent>, PostLikedEventHandler>();
        // services.AddScoped<IEventHandler<MessageUpdatedEvent>, MessageUpdatedEventHandler>();
        // services.AddScoped<IEventHandler<PostRemovedEvent>, PostRemovedEventHandler>();
        // services.AddScoped<IEventHandler<CommentAddedEvent>, CommentAddedEventHandler>();
        // services.AddScoped<IEventHandler<CommentRemovedEvent>, CommentRemovedEventHandler>();
        // services.AddScoped<IEventHandler<CommentUpdatedEvent>, CommentUpdatedEventHandler>();

        services.AddScoped<IEventConsumer, EventConsumer>();
        return services;
    }

    private static bool IsClass(Type t) => !t.IsAbstract && !t.IsInterface;

    private static IEnumerable<Type> GetEnumerableOfType()
    {
        return Assembly.GetExecutingAssembly().GetTypes().Where(t =>
            t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEventHandler<>)));
    }
}