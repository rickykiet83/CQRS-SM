namespace CQRS.Core.Messages;

public abstract record Message
{
    public Guid Id { get; set; }
}