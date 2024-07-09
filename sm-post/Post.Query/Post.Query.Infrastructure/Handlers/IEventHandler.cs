using CQRS.Core.Events;

namespace Post.Query.Infrastructure.Handlers;

public interface IEventHandler<TEvent> where TEvent: BaseEvent
{
    Task Handler(TEvent? @event);
}