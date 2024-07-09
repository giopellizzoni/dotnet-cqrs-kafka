namespace CQRS.Core.Messages;

public interface IMessage
{
    Guid Id { get; init; }
}