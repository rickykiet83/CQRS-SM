namespace Post.Common.Events;

public record PostLikedEvent(Guid id) : BaseEvent(nameof(PostLikedEvent))
{
}