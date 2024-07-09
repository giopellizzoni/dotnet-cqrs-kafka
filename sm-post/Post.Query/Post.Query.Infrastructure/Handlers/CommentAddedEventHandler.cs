using Ardalis.GuardClauses;
using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public sealed record CommentAddedEventHandler(ICommentRepository commentRepository) : IEventHandler<CommentAddedEvent>
{

    public async Task Handler(CommentAddedEvent? @event)
    {
        Guard.Against.Null(@event);
        
        var comment = new CommentEntity
        {
            PostId = @event.Id,
            CommentId = @event.CommentId,
            CommentDate = @event.CommentDate,
            Comment = @event.Comment,
            Username = @event.Username,
            Edited = false
        };
        await commentRepository.CreateAsync(comment);
    }
}