using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class PostCreatedEventHandler(IPostRepository postRepository) : IEventHandler<PostCreatedEvent>
{
    public event EventHandler<PostCreatedEvent>? On;

    public async Task Handler(PostCreatedEvent @event)
    {
        var post = new PostEntity
        {
            PostId = @event.Id,
            Author = @event.Author,
            DatePosted = @event.DatePosted,
            Message = @event.Message
        };
        await postRepository.CreateAsync(post);
    }
}