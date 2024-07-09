using CQRS.Core.Messages;

namespace CQRS.Core.Commands;

public abstract record BaseCommand(Guid Id) : IMessage;