using Post.Common.Events;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;

namespace Post.Query.Infrastructure.Handlers;

public class EventHandler(IPostRepository postRepository, ICommentRepository commentRepository) : IEventHandler
{
    public async Task On(PostCreatedEvent @event)
    {
        var post = new PostEntity
        {
            PostId = @event.Id,
            Author = @event.Author,
            Message = @event.Message,
        };

        await postRepository.CreateAsync(post);
    }

    public async Task On(MessageUpdatedEvent @event)
    {
        var post = await postRepository.GetByIdAsync(@event.Id);
        if (post is null) return;

        post.Message = @event.Message;

        await postRepository.UpdateAsync(post);
    }

    public async Task On(PostLikedEvent @event)
    {
        var post = await postRepository.GetByIdAsync(@event.Id);
        if (post is null) return;

        post.Likes++;

        await postRepository.UpdateAsync(post);
    }

    public async Task On(CommentAddedEvent @event)
    {
        var post = await postRepository.GetByIdAsync(@event.Id);
        if (post is null) return;

        var comment = new CommentEntity
        {
            CommentId = @event.CommentId,
            PostId = @event.Id,
            Username = @event.Username,
            Comment = @event.Comment,
        };

        await commentRepository.CreateAsync(comment);
    }

    public async Task On(CommentUpdatedEvent @event)
    {
        var post = await postRepository.GetByIdAsync(@event.Id);

        var comment = post?.Comments.FirstOrDefault(x => x.CommentId == @event.CommentId);
        if (comment is null) return;

        comment.Comment = @event.Comment;

        await commentRepository.UpdateAsync(comment);
    }

    public async Task On(CommentRemovedEvent @event)
    {
        var post = await postRepository.GetByIdAsync(@event.Id);

        var comment = post?.Comments.FirstOrDefault(x => x.CommentId == @event.CommentId);
        if (comment is null) return;

        await commentRepository.DeleteAsync(comment.CommentId);
    }
}