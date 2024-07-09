using Ardalis.GuardClauses;
using Post.Common.Events;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public sealed record CommentRemovedEventHandler(ICommentRepository commentRepository) : IEventHandler<CommentRemovedEvent>
{
    public async Task Handler(CommentRemovedEvent? @event)
    {
        Guard.Against.Null(@event);
        await commentRepository.DeleteAsync(@event.CommentId);
    }
}