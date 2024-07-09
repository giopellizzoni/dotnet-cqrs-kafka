using Ardalis.GuardClauses;
using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public sealed record PostLikedEventHandler(IPostRepository postRepository) : IEventHandler<PostLikedEvent>
{

    public async Task Handler(PostLikedEvent? @event)
    {
        Guard.Against.Null(@event);
        
        var post = await postRepository.GetByIdAsync(@event.Id);
        if (post == null) return;

        post.Likes++;
        await postRepository.UpdateAsync(post);
    }
}