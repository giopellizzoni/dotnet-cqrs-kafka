using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Application.Commands.Comments.AddComment;

public class AddCommentCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    : IRequestHandler<AddCommentCommand>
{
    public async Task<Unit> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(request.Id);
        aggregate.AddComment(request.Comment, request.Username);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}