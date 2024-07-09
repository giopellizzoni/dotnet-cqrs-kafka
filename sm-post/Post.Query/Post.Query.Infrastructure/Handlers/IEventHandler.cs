using CQRS.Core.Events;

namespace Post.Query.Infrastructure.Handlers;

public interface IEventHandler<TEvent> where TEvent: BaseEvent
{
    event EventHandler<TEvent> On;
    Task Handler(TEvent eventArgs);
}