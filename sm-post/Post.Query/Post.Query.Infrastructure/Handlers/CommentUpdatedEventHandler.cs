using Ardalis.GuardClauses;
using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public sealed record CommentUpdatedEventHandler(ICommentRepository commentRepository) : IEventHandler<CommentUpdatedEvent>
{
    public async Task Handler(CommentUpdatedEvent? @event)
    {

        Guard.Against.Null(@event);
        
        var comment = await commentRepository.GetByIdAsync(@event.CommentId);
        if (comment == null) return;
        comment.Comment = @event.Comment;
        comment.Edited = true;
        comment.CommentDate = @event.EditDate;

        await commentRepository.UpdateAsync(comment);
    }
}