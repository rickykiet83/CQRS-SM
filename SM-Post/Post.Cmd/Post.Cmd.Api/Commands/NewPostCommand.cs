namespace Post.Cmd.Api.Commands;

public record NewPostCommand : BaseCommand
{
    public new Guid Id { get; init; } = Guid.NewGuid();
    public string Author { get; set; }
    public string Message { get; set; }
}