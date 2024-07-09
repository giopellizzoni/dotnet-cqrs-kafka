using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class CommentRemovedEventHandler(ICommentRepository commentRepository) : IEventHandler<CommentRemovedEvent>
{

    public event EventHandler<CommentRemovedEvent>? On;
    public async Task Handler(CommentRemovedEvent eventArgs)
    {
        await commentRepository.DeleteAsync(eventArgs.CommentId);
    }
}