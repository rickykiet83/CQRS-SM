using CQRS.Core.Domain;
using Post.Common.Events;

namespace Post.Cmd.Domain.Aggregates;

public class PostAggregate : AggregateRoot
{
    private bool _active;
    private string _author;
    private readonly Dictionary<Guid, Tuple<string, string>> _comments = new();

    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    public PostAggregate()
    {
    }

    public PostAggregate(Guid id, string author, string message)
    {
        RaiseEvent(new PostCreatedEvent
        {
            Id = id,
            Author = author,
            Message = message,
            DatePosted = DateTime.UtcNow,
        });
    }

    public void Apply(PostCreatedEvent @event)
    {
        _id = @event.Id;
        _active = true;
        _author = @event.Author;
    }

    public void EditMessage(string message)
    {
        if (!_active)
            throw new InvalidOperationException("You can't edit a message of an inactive post.");

        if (string.IsNullOrWhiteSpace(message))
            throw new InvalidOperationException(
                $"The value of {nameof(message)} can't be null or empty. Please provide a valid {nameof(message)}.");

        RaiseEvent(new MessageUpdatedEvent(message) { Id = _id });
    }

    public void Apply(MessageUpdatedEvent @event)
    {
        _id = @event.Id;
    }

    public void LikePost()
    {
        if (!_active)
            throw new InvalidOperationException("You can't like an inactive post.");

        RaiseEvent(new PostLikedEvent { Id = _id });
    }

    public void Apply(PostLikedEvent @event)
    {
        _id = @event.Id;
    }

    public void AddComment(string comment, string username)
    {
        if (!_active)
            throw new InvalidOperationException("You can't add a comment to an inactive post.");

        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException(
                $"The value of {nameof(username)} can't be null or empty. Please provide a valid {nameof(username)}.");

        if (string.IsNullOrWhiteSpace(comment))
            throw new InvalidOperationException(
                $"The value of {nameof(comment)} can't be null or empty. Please provide a valid {nameof(comment)}.");

        RaiseEvent(new CommentAddedEvent
        {
            Id = _id,
            CommentId = Guid.NewGuid(),
            Comment = comment,
            Username = username,
            CommentDate = DateTime.UtcNow,
        });
    }

    public void Apply(CommentAddedEvent @event)
    {
        if (_comments.ContainsKey(@event.CommentId))
            return;
        
        _id = @event.Id;
        _comments.Add(@event.CommentId, new Tuple<string, string>(@event.Comment, @event.Username));
    }

    public void EditComment(Guid commentId, string comment, string username)
    {
        if (!_active)
            throw new InvalidOperationException("You can't edit a comment of an inactive post.");

        if (string.IsNullOrWhiteSpace(comment))
            throw new InvalidOperationException(
                $"The value of {nameof(comment)} can't be null or empty. Please provide a valid {nameof(comment)}.");

        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You can't edit a comment that you didn't create.");

        RaiseEvent(new CommentUpdatedEvent
        {
            Id = _id,
            CommentId = commentId,
            Comment = comment,
            Username = username,
            EditDate = DateTime.UtcNow,
        });
    }

    public void Apply(CommentUpdatedEvent @event)
    {
        _id = @event.Id;
        _comments[@event.CommentId] = new Tuple<string, string>(@event.Comment, @event.Username);
    }

    public void RemoveComment(Guid commentId, string username)
    {
        if (!_active)
            throw new InvalidOperationException("You can't remove a comment of an inactive post.");

        if (!_comments[commentId].Item2.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You can't remove a comment that you didn't create.");

        RaiseEvent(new CommentRemovedEvent(commentId) { Id = _id });
    }

    public void Apply(CommentRemovedEvent @event)
    {
        _id = @event.Id;
        _comments.Remove(@event.CommentId);
    }

    public void DeletePost(string username)
    {
        if (!_active)
            throw new InvalidOperationException("You can't delete an inactive post.");

        if (_author.Equals(username, StringComparison.CurrentCultureIgnoreCase))
            throw new InvalidOperationException("You can't delete a post that you didn't create.");

        RaiseEvent(new PostRemovedEvent { Id = _id });
    }
    
    public void Apply(PostRemovedEvent @event)
    {
        _id = @event.Id;
        _active = false;
    }
}