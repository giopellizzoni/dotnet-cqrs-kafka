using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Application.Commands.RestoreDb;

public class RestoreReadDbCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    : IRequestHandler<RestoreReadDbCommand>
{
    public async Task<Unit> Handle(RestoreReadDbCommand request, CancellationToken cancellationToken)
    {
        await eventSourcingHandler.RepublishEventsAsync();
        return Unit.Value;
    }
}