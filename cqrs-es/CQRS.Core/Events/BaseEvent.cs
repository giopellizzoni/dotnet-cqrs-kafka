using CQRS.Core.Messages;

namespace CQRS.Core.Events
{
    public abstract class BaseEvent : IMessage, IComparable<BaseEvent>
    {

        protected BaseEvent(string type)
        {
            Type = type;
        }

        public int Version { get; set; }
        public string Type { get; set; }

        public Guid Id { get; init; }

        public int CompareTo(BaseEvent? other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var versionComparison = Version.CompareTo(other.Version);
            if (versionComparison != 0) return versionComparison;
            var typeComparison = string.Compare(Type, other.Type, StringComparison.Ordinal);
            if (typeComparison != 0) return typeComparison;
            return Id.CompareTo(other.Id);
        }
    }
}