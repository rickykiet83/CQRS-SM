namespace Post.Common.Events;

public record CommentUpdatedEvent() : BaseEvent(nameof(CommentUpdatedEvent))
{
    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string Username { get; set; }
    public DateTime EditDate { get; set; }
}