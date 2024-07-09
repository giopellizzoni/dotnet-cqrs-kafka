using Confluent.Kafka;
using CQRS.Core.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using Post.Cmd.Infrastructure.Config;
using Post.Common.Events;

namespace Post.Cmd.Infrastructure;

public static class InfrastructureModule
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfigurationManager configuration)
    {

        services.AddMongo(configuration)
            .AddProducer(configuration);
        return services;
    }


    private static IServiceCollection AddMongo(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.Configure<MongoDbConfig>(configuration.GetSection(nameof(MongoDbConfig)));
        services.Configure<MongoDbConfig>(configuration);

        RegisterMongoBsonClasses();
        
        return services;
    }


    private static IServiceCollection AddProducer(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.Configure<ProducerConfig>(configuration.GetSection(nameof(ProducerConfig)));

        return services;
    }

    private static void RegisterMongoBsonClasses()
    {
        BsonClassMap.RegisterClassMap<BaseEvent>();
        BsonClassMap.RegisterClassMap<PostCreatedEvent>();
        BsonClassMap.RegisterClassMap<MessageUpdatedEvent>();
        BsonClassMap.RegisterClassMap<PostLikedEvent>();
        BsonClassMap.RegisterClassMap<CommentAddedEvent>();
        BsonClassMap.RegisterClassMap<CommentUpdatedEvent>();
        BsonClassMap.RegisterClassMap<CommentRemovedEvent>();
        BsonClassMap.RegisterClassMap<PostRemovedEvent>();
    }
}