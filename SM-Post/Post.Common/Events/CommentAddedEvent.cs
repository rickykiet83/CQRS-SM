namespace Post.Common.Events;

public record CommentAddedEvent() : BaseEvent(nameof(CommentAddedEvent))
{
    public Guid CommentId { get; set; }
    public string Comment { get; set; }
    public string Username { get; set; }
    public DateTime CommentDate { get; set; }
}