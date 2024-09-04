namespace Post.Common.Events;

public record MessageUpdatedEvent(Guid Id, string Message) : BaseEvent(nameof(MessageUpdatedEvent))
{
}