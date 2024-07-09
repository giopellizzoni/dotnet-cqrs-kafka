using CQRS.Core.Commands;
using CQRS.Core.Messages;
using MediatR;

namespace Post.Cmd.Application.Commands.Posts.NewPost;

public record NewPostCommand(string Author, string Message) : BaseCommand(Guid.Empty), IRequest;