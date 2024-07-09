using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Application.Commands.Comments.EditComment;

public class EditCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    : IRequestHandler<EditCommentCommand>
{
    public async Task<Unit> Handle(EditCommentCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(request.Id);
        aggregate.EditComment(request.CommentId, request.Comment, request.Username);
        await eventSourcingHandler.SaveAsync(aggregate);

        return Unit.Value;
    }
}