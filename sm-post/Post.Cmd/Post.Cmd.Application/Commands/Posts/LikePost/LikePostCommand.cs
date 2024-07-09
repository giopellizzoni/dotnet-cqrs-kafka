using CQRS.Core.Commands;
using CQRS.Core.Messages;
using MediatR;

namespace Post.Cmd.Application.Commands.Posts.LikePost;

public record LikePostCommand(Guid Id) : BaseCommand(Id), IRequest;