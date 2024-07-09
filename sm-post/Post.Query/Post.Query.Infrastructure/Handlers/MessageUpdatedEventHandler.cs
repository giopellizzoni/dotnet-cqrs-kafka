using Ardalis.GuardClauses;
using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public sealed record MessageUpdatedEventHandler(IPostRepository postRepository) : IEventHandler<MessageUpdatedEvent>
{
    public async Task Handler(MessageUpdatedEvent? @event)
    {
        Guard.Against.Null(@event);
        var post = await postRepository.GetByIdAsync(@event.Id);
        if (post == null) return;

        post.Message = @event.Message;

        await postRepository.UpdateAsync(post);
    }
}