using CQRS.Core.Commands;
using CQRS.Core.Messages;
using MediatR;

namespace Post.Cmd.Application.Commands.Comments.EditComment;

public record EditCommentCommand(Guid CommentId, string Comment, string Username) : BaseCommand(Guid.Empty), IRequest;