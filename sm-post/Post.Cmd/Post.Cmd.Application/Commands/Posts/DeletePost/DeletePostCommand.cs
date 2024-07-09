using CQRS.Core.Commands;
using CQRS.Core.Messages;
using MediatR;

namespace Post.Cmd.Application.Commands.Posts.DeletePost;

public record DeletePostCommand(string Username) : BaseCommand(Guid.Empty), IRequest;