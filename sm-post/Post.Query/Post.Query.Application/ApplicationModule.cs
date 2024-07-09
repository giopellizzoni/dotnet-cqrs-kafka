using System.Reflection;
using CQRS.Core.Consumers;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Post.Query.Application.Queries.FindAllPosts;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.Consumers;
using Post.Query.Infrastructure.Handlers;
using Post.Query.Infrastructure.Repositories;

namespace Post.Query.Application;

public static class ApplicationModule
{

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddMediatR()
            .AddDependencyInjection();
        
        return services;
    }


    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatR(typeof(FindAllPostsQuery));
        return services;
    }

    private static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        
        
        
        return services;
    }

  

}