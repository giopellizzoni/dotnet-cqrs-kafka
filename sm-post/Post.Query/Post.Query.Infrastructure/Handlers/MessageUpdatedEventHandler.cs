using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class MessageUpdatedEventHandler(IPostRepository postRepository) : IEventHandler<MessageUpdatedEvent>
{
    public event EventHandler<MessageUpdatedEvent>? On;

    public async Task Handler(MessageUpdatedEvent eventArgs)
    {
        var post = await postRepository.GetByIdAsync(eventArgs.Id);
        if (post == null) return;

        post.Message = eventArgs.Message;

        await postRepository.UpdateAsync(post);
    }
}