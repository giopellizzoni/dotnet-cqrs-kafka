using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class PostLikedEventHandler(IPostRepository postRepository) : IEventHandler<PostLikedEvent>
{
    public event EventHandler<PostLikedEvent>? On;

    public async Task Handler(PostLikedEvent eventArgs)
    {
        var post = await postRepository.GetByIdAsync(eventArgs.Id);
        if (post == null) return;

        post.Likes++;
        await postRepository.UpdateAsync(post);
    }
}