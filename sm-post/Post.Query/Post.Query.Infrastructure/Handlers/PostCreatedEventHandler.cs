using Ardalis.GuardClauses;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public sealed record PostCreatedEventHandler(IPostRepository postRepository) : IEventHandler<PostCreatedEvent>
{
    public async Task Handler(PostCreatedEvent? @event)
    {
        Guard.Against.Null(@event);
        
        var post = new PostEntity
        {
            PostId = @event.Id,
            Author = @event.Author,
            DatePosted = @event.DatePosted,
            Message = @event.Message
        };
        await postRepository.CreateAsync(post);
    }

}