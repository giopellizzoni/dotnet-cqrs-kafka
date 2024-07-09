using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class CommentAddedEventHandler(ICommentRepository commentRepository) : IEventHandler<CommentAddedEvent>
{

    public event EventHandler<CommentAddedEvent>? On;
    public async Task Handler(CommentAddedEvent eventArgs)
    {
        var comment = new CommentEntity
        {
            PostId = eventArgs.Id,
            CommentId = eventArgs.CommentId,
            CommentDate = eventArgs.CommentDate,
            Comment = eventArgs.Comment,
            Username = eventArgs.Username,
            Edited = false
        };
        await commentRepository.CreateAsync(comment);
    }
}