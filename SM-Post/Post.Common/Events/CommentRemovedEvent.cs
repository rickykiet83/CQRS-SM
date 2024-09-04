namespace Post.Common.Events;

public record CommentRemovedEvent() : BaseEvent(nameof(CommentRemovedEvent))
{
    public Guid CommentId { get; set; }
}