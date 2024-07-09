namespace CQRS.Core.Exceptions;

public class EventStreamNotFound : Exception
{
    public EventStreamNotFound(string message): base(message)
    {
        
    }   
}