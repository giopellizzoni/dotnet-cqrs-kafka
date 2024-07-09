using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class PostRemovedEventHandler(IPostRepository postRepository) : IEventHandler<PostRemovedEvent>
{
    public event EventHandler<PostRemovedEvent>? On;
    public async Task Handler(PostRemovedEvent eventArgs)
    {
        await postRepository.DeleteAsync(eventArgs.Id);
    }
}