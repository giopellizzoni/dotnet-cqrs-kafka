using Ardalis.GuardClauses;
using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public sealed class PostRemovedEventHandler(IPostRepository postRepository) : IEventHandler<PostRemovedEvent>
{
    public async Task Handler(PostRemovedEvent? @event)
    {
        Guard.Against.Null(@event);
        await postRepository.DeleteAsync(@event.Id);
    }
}