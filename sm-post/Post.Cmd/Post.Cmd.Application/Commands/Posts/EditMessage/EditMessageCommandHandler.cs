using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Application.Commands.Posts.EditMessage;

public class EditMessageCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    : IRequestHandler<EditMessageCommand>
{
    public async Task<Unit> Handle(EditMessageCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(request.Id);
        aggregate.EditMessage(request.Message);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}