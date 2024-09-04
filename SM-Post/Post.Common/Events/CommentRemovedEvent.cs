namespace Post.Common.Events;

public record CommentRemovedEvent(Guid CommentId) : BaseEvent(nameof(CommentRemovedEvent))
{
}