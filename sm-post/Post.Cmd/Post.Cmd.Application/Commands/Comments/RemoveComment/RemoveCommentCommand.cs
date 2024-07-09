using CQRS.Core.Commands;
using CQRS.Core.Messages;
using MediatR;

namespace Post.Cmd.Application.Commands.Comments.RemoveComment;

public record RemoveCommentCommand(Guid CommentId, string Username) : BaseCommand(Guid.NewGuid()), IRequest;