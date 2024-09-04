using CQRS.Core.Messages;

namespace CQRS.Core.Events;

public abstract record BaseEvent : Message
{
    protected BaseEvent(string type)
    {
        Type = type;
    }
    
    public int Version { get; set; }
    public string Type { get; set; }
}