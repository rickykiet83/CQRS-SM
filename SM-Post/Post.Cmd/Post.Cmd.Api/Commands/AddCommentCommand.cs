namespace Post.Cmd.Api.Commands;

public record AddCommentCommand : BaseCommand
{
    public string Comment { get; set; }
    public string Username { get; set; }
}