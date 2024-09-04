namespace Post.Common.Events;

public record PostCreatedEvent() : BaseEvent(nameof(PostCreatedEvent))
{
    public string Author { get; set; }
    public string Message { get; set; }
    public DateTime DatePosted { get; set; }
}