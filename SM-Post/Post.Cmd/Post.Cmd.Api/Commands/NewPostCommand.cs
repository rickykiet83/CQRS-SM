namespace Post.Cmd.Api.Commands;

public record NewPostCommand : BaseCommand
{
    public string Author { get; set; }
    public string Message { get; set; }
}