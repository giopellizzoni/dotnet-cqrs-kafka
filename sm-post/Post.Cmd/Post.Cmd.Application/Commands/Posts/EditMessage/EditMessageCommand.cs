using CQRS.Core.Commands;
using CQRS.Core.Messages;
using MediatR;

namespace Post.Cmd.Application.Commands.Posts.EditMessage;
public record EditMessageCommand(string Message) : BaseCommand(Guid.Empty), IRequest;