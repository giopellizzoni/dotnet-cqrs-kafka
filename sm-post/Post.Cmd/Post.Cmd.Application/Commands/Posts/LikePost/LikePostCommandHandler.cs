using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Application.Commands.Posts.LikePost;

public class LikePostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    : IRequestHandler<LikePostCommand>
{
    public async Task<Unit> Handle(LikePostCommand request, CancellationToken cancellationToken)
    {
        var aggregate = await eventSourcingHandler.GetByIdAsync(request.Id);
        aggregate.LikePost();
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}