using CQRS.Core.Messages;

namespace CQRS.Core.Events
{
    public abstract class BaseEvent : IMessage
    {
        protected BaseEvent(string type)
        {
            Type = type;
        }

        public int Version { get; set; }
        public string Type { get; set; }

        public Guid Id { get; init; }

        protected BaseEvent()
        {
        }
    }
}