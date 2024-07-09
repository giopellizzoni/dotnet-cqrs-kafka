using CQRS.Core.Handlers;
using MediatR;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Application.Commands.Posts.NewPost;

public class NewPostCommandHandler(IEventSourcingHandler<PostAggregate> eventSourcingHandler)
    : IRequestHandler<NewPostCommand>
{
    public async Task<Unit> Handle(NewPostCommand request, CancellationToken cancellationToken)
    {
        var aggregate = new PostAggregate(request.Id, request.Author, request.Message);
        await eventSourcingHandler.SaveAsync(aggregate);
        return Unit.Value;
    }
}