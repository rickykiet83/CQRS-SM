namespace Post.Cmd.Api.Commands;

public record DeletePostCommand : BaseCommand
{
    public string Username { get; set; }
}