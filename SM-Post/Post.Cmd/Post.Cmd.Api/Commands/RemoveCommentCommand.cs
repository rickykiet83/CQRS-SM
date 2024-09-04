namespace Post.Cmd.Api.Commands;

public record RemoveCommentCommand : BaseCommand
{
    public Guid CommentId { get; set; }
    public string Username { get; set; }
}