using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Application.Commands.Posts.DeletePost;

public class DeletePostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    : IRequestHandler<DeletePostCommand>
{
    public async Task<Unit> Handle(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(request.Id);
        aggregate.DeletePost(request.Username);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}