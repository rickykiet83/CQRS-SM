namespace Post.Cmd.Api.Commands;

public record DeletePostCommand : BaseCommand
{
    public Guid PostId { get; set; }
    public string Username { get; set; }
}