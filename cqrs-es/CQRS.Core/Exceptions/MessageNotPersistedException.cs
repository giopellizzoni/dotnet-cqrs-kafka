namespace CQRS.Core.Exceptions;

public class MessageNotPersistedException : Exception
{
    public MessageNotPersistedException(string message) : base(message)
    {
        
    }
}