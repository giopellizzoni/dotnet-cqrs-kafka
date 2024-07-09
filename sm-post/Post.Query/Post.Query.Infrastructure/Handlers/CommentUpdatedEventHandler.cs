using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class CommentUpdatedEventHandler(ICommentRepository commentRepository) : IEventHandler<CommentUpdatedEvent>
{
    public event EventHandler<CommentUpdatedEvent>? On;

    public async Task Handler(CommentUpdatedEvent eventArgs)
    {
        var comment = await commentRepository.GetByIdAsync(eventArgs.CommentId);
        if (comment == null) return;
        comment.Comment = eventArgs.Comment;
        comment.Edited = true;
        comment.CommentDate = eventArgs.EditDate;

        await commentRepository.UpdateAsync(comment);
    }
}