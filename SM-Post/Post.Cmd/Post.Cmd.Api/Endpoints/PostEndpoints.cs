using CQRS.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;
using Post.Common.Exceptions;

namespace Post.Cmd.Api.Endpoints;

public static class PostEndpoints
{
    public static RouteGroupBuilder MapPostEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/api/v1/posts")
            .WithParameterValidation()
            .WithTags("Posts");

        group.MapPost("/", async (
                ILogger<RouteGroupBuilder> logger,
                [FromBody] NewPostCommand command, ICommandDispatcher dispatcher) =>
            {
                try
                {
                    await dispatcher.SendAsync(command);
                    return Results.Created("",
                        new NewPostResponse(id: command.Id,
                            message: "New post creation request completed successfully"));
                }
                catch (InvalidOperationException ex)
                {
                    logger.Log(LogLevel.Warning, ex, "Error creating new post");
                    throw new BadRequestException("Error creating new post", ex.Message);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, ex, "Error creating new post");
                    throw new InternalServerException("Error creating new post", ex.Message);
                }
            })
            .WithSummary("Creates a new post")
            .WithDescription("Creates a new post with the specified details")
            .Produces<NewPostResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            ;

        group.MapPut("/{id:guid}/like", async (
                Guid id,
                ILogger<RouteGroupBuilder> logger,
                ICommandDispatcher dispatcher) =>
            {
                try
                {
                    await dispatcher.SendAsync(new LikePostCommand { Id = id });
                    return Results.Ok(new BaseResponse { Message = "Post liked successfully" });
                }
                catch (InvalidOperationException ex)
                {
                    logger.Log(LogLevel.Warning, ex, "Error liking post");
                    throw new BadRequestException("Error liking post", ex.Message);
                }
                catch (AggregateNotFoundException ex)
                {
                    logger.Log(LogLevel.Warning, ex, "No post found");
                    throw new NotFoundException("Error No post found", ex.Message);
                }
                catch (Exception ex)
                {
                    const string message = "Error while like post request";
                    logger.Log(LogLevel.Error, ex, message);
                    throw new InternalServerException(ex.Message);
                }
            }).WithSummary("Likes a post")
            .WithDescription("Likes a post with the specified details")
            .Produces<BaseResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        
        group.MapPost("/{id:guid}/comments", async (
                Guid id,
                ILogger<RouteGroupBuilder> logger,
                [FromBody] AddCommentCommand command, ICommandDispatcher dispatcher) =>
            {
                try
                {
                    command.Id = id;
                    await dispatcher.SendAsync(command);
                    return Results.Created("",
                        new BaseResponse{ Message = "New comment creation request completed successfully" });
                }
                catch (InvalidOperationException ex)
                {
                    logger.Log(LogLevel.Warning, ex, "Error creating new comment");
                    throw new BadRequestException("Error creating new comment", ex.Message);
                }
                catch (AggregateNotFoundException ex)
                {
                    logger.Log(LogLevel.Warning, ex, "No post found");
                    throw new NotFoundException("Error No post found", ex.Message);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, ex, "Error creating new comment");
                    throw new InternalServerException("Error creating new comment", ex.Message);
                }
            })
            .WithSummary("Creates a new comment")
            .WithDescription("Creates a new comment with the specified details")
            .Produces<BaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            ;

        group.MapPut("/{id:guid}/comments/{commentId:guid}", async (
            Guid id,
            Guid commentId,
            ILogger<RouteGroupBuilder> logger,
            [FromBody] EditCommentCommand command, ICommandDispatcher dispatcher) =>
        {
            try
            {
                command.Id = id;
                command.CommentId = commentId;
                await dispatcher.SendAsync(command);
                return Results.Ok(
                    new BaseResponse{ Message = "Edit comment successfully" });
            }
            catch (InvalidOperationException ex)
            {
                logger.Log(LogLevel.Warning, ex, "Error Edit comment");
                throw new BadRequestException("Error Edit comment", ex.Message);
            }
            catch (AggregateNotFoundException ex)
            {
                logger.Log(LogLevel.Warning, ex, "No post or comment found");
                throw new NotFoundException("No post or comment", ex.Message);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, ex, "Error Edit comment");
                throw new InternalServerException("Error Edit comment", ex.Message);
            }
        }).WithSummary("Edit a comment")
        .WithDescription("Edit a comment with the specified details")
        .Produces<BaseResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest);
        
        return group;
    }
}