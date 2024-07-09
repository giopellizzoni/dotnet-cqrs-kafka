using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Application.Commands.Comments.RemoveComment;

public class RemoveCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    : IRequestHandler<RemoveCommentCommand>
{
    public async Task<Unit> Handle(RemoveCommentCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(request.Id);
        aggregate.RemoveComment(request.CommentId, request.Username);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}