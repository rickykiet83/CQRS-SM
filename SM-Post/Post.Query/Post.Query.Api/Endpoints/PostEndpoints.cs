using CQRS.Core.Infrastructure;
using Post.Common.Exceptions;
using Post.Query.Api.DTOs;
using Post.Query.Api.Queries;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.Endpoints;

public static class PostEndpoints
{
    public static RouteGroupBuilder MapPostEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/api/v1/posts")
            .WithParameterValidation()
            .WithTags("Posts");

        group.MapGet("/", async (ILogger<RouteGroupBuilder> logger,
                IQueryDispatcher<PostEntity> dispatcher
            ) =>
            {
                try
                {
                    var posts = await dispatcher.SendAsync(new FindAllPostsQuery());
                    if (posts is null || !posts.Any())
                        return Results.NoContent();

                    var count = posts.Count;
                    return Results.Ok(new PostLookupResponse
                    {
                        Posts = posts,
                        Message = $"Successfully retrieved {count} post{(count > 1 ? "s" : string.Empty)}"
                    });
                }
                catch (Exception ex)
                {
                    const string message = "Error while processing request to retrieve all posts";
                    logger.Log(LogLevel.Error, ex, message);
                    throw new InternalServerException(message, ex.Message);
                }
            })
            .WithSummary("Get all posts")
            .WithDescription("Get all posts")
            .Produces<List<PostEntity>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", async (
                Guid id,
                ILogger<RouteGroupBuilder> logger,
                IQueryDispatcher<PostEntity> dispatcher
            ) =>
            {
                try
                {
                    var posts = await dispatcher.SendAsync(new FindPostByIdQuery(id));
                    if (posts is null || !posts.Any())
                        throw new NotFoundException("post", id);

                    return Results.Ok(new PostLookupResponse
                    {
                        Posts = posts,
                        Message = $"Successfully retrieved post with id {id}"
                    });
                }
                catch (Exception ex)
                {
                    const string message = "Error while processing request to retrieve a post";
                    logger.Log(LogLevel.Error, ex, message);
                    throw new InternalServerException(message, ex.Message);
                }
            })
            .WithSummary("Get a post by id")
            .WithDescription("Get a post by id")
            .Produces<List<PostEntity>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        group.MapGet("/authors/{author}", async (
                string author,
                ILogger<RouteGroupBuilder> logger,
                IQueryDispatcher<PostEntity> dispatcher
            ) =>
            {
                try
                {
                    var posts = await dispatcher.SendAsync(new FindPostsByAuthorQuery { Author = author});
                    if (posts is null)
                        Results.NoContent();

                    return Results.Ok(new PostLookupResponse
                    {
                        Posts = posts,
                        Message = $"Successfully retrieved post with author {author}"
                    });
                }
                catch (Exception ex)
                {
                    const string message = "Error while processing request to retrieve all posts of author";
                    logger.Log(LogLevel.Error, ex, message);
                    throw new InternalServerException(message, ex.Message);
                }
            })
            .WithSummary("Get all posts by author")
            .WithDescription("Get all posts by author")
            .Produces<List<PostEntity>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        group.MapGet("/withComments", async (
                ILogger<RouteGroupBuilder> logger,
                IQueryDispatcher<PostEntity> dispatcher
            ) =>
            {
                try
                {
                    var posts = await dispatcher.SendAsync(new FindPostsWithCommentsQuery());
                    if (posts is null)
                        Results.NoContent();

                    return Results.Ok(new PostLookupResponse
                    {
                        Posts = posts,
                        Message = $"Successfully retrieved post with comments"
                    });
                }
                catch (Exception ex)
                {
                    const string message = "Error while processing request to retrieve all posts with comments";
                    logger.Log(LogLevel.Error, ex, message);
                    throw new InternalServerException(message, ex.Message);
                }
            })
            .WithSummary("Get all posts with comments")
            .WithDescription("Get all posts with comments")
            .Produces<List<PostEntity>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
        
        group.MapGet("/withLikes/{numberOfLikes:int}", async (
                int numberOfLikes,
                ILogger<RouteGroupBuilder> logger,
                IQueryDispatcher<PostEntity> dispatcher
            ) =>
            {
                try
                {
                    var posts = await dispatcher.SendAsync(new FindPostsWithLikesQuery {NumberOfLikes = numberOfLikes});
                    if (posts is null)
                        Results.NoContent();

                    return Results.Ok(new PostLookupResponse
                    {
                        Posts = posts,
                        Message = $"Successfully retrieved post with number of likes {numberOfLikes}"
                    });
                }
                catch (Exception ex)
                {
                    const string message = "Error while processing request to retrieve all posts with number of likes";
                    logger.Log(LogLevel.Error, ex, message);
                    throw new InternalServerException(message, ex.Message);
                }
            })
            .WithSummary("Get all posts with number of likes")
            .WithDescription("Get all posts with number of likes")
            .Produces<List<PostEntity>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        return group;
    }
}