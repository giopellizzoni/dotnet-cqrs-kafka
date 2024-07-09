using CQRS.Core.Commands;
using CQRS.Core.Messages;
using MediatR;

namespace Post.Cmd.Application.Commands.Comments.AddComment;

public record AddCommentCommand(string Comment, string Username) : BaseCommand(Guid.Empty), IRequest;
