using CQRS.Core.Events;

namespace CQRS.Core.Domain;

public abstract class AggregateRoot
{
    protected Guid _id;
    public Guid Id => _id;
    private readonly List<BaseEvent> _changes = new();

    public int Version { get; set; } = -1;

    public IEnumerable<BaseEvent> GetUncommittedChanges() => _changes;

    public void MarkChangesAsCommitted() => _changes.Clear();

    public void ApplyChange(BaseEvent @event, bool isNew)
    {
        var method = GetType().GetMethod("Apply", new Type[] { @event.GetType() });
        if (method is null)
            throw new ArgumentNullException(nameof(method),
                $"The Apply method for {@event.GetType().Name} is not implemented in {GetType().Name}");

        method.Invoke(this, new object[] { @event });
        
        if (isNew)
            _changes.Add(@event);
    }
    
    protected void RaiseEvent(BaseEvent @event)
    {
        ApplyChange(@event, true);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events)
            ApplyChange(@event, false);
    }
}