using Microsoft.EntityFrameworkCore;
using Post.Query.Domain.Entities;
using Post.Query.Domain.Repositories;
using Post.Query.Infrastructure.DataAccess;

namespace Post.Query.Infrastructure.Repositories;

public class PostRepository(DatabaseContextFactory contextFactory) : IPostRepository
{
    public async Task CreateAsync(PostEntity? post)
    {
        await using DatabaseContext context = contextFactory.CreateDbContext();
        context.Posts.Add(post);

        _ = await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid postId)
    {
        await using DatabaseContext context = contextFactory.CreateDbContext();
        var post = await GetByIdAsync(postId);

        if (post == null) return;

        context.Posts.Remove(post);
        _ = await context.SaveChangesAsync();
    }

    public async Task<List<PostEntity>> ListByAuthorAsync(string author)
    {
        await using DatabaseContext context = contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(i => i.Comments).AsNoTracking()
            .Where(x => x.Author.Contains(author))
            .ToListAsync();
    }

    public async Task<PostEntity?> GetByIdAsync(Guid postId)
    {
        await using DatabaseContext context = contextFactory.CreateDbContext();
        return await context.Posts
            .Include(i => i.Comments)
            .FirstOrDefaultAsync(x => x.PostId == postId);
    }

    public async Task<List<PostEntity>> ListAllAsync()
    {
        await using DatabaseContext context = contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(i => i.Comments).AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithCommentsAsync()
    {
        await using DatabaseContext context = contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(i => i.Comments).AsNoTracking()
            .Where(x => x.Comments != null && x.Comments.Any())
            .ToListAsync();
    }

    public async Task<List<PostEntity>> ListWithLikesAsync(int numberOfLikes)
    {
        await using DatabaseContext context = contextFactory.CreateDbContext();
        return await context.Posts.AsNoTracking()
            .Include(i => i.Comments).AsNoTracking()
            .Where(x => x.Likes >= numberOfLikes)
            .ToListAsync();
    }

    public async Task UpdateAsync(PostEntity? post)
    {
        await using DatabaseContext context = contextFactory.CreateDbContext();
        context.Posts.Update(post);

        _ = await context.SaveChangesAsync();
    }
}