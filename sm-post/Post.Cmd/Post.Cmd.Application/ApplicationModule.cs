using CQRS.Core.Domain;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Post.Cmd.Application.Commands.Posts.NewPost;
using Post.Cmd.Domain.Aggregates;
using Post.Cmd.Infrastructure.Handlers;
using Post.Cmd.Infrastructure.Producers;
using Post.Cmd.Infrastructure.Repositories;
using Post.Cmd.Infrastructure.Stores;

namespace Post.Cmd.Application;

public static class ApplicationModule
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(NewPostCommand))
            .AddDependencyInjection();

        return services;
    }

    private static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<IEventSourcingHandler<PostAggregate>, EventSourcingHandler>();
        services.AddScoped<IEventStoreRepository, EventStoreRepository>();
        services.AddScoped<IEventStore, EventStore>();
        services.AddScoped<IEventProducer, EventProducer>();
        
        return services;
    }
}