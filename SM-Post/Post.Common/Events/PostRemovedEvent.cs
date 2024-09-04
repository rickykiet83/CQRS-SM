namespace Post.Common.Events;

public record PostRemovedEvent() : BaseEvent(nameof(PostRemovedEvent))
{
}