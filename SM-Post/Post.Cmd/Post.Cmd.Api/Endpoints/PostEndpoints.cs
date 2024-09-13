using CQRS.Core.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;
using Zip.WebAPI.Exceptions;

namespace Post.Cmd.Api.Endpoints;

public static class PostEndpoints
{
    public static RouteGroupBuilder MapPostEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/api/v1/posts")
            .WithParameterValidation()
            .WithTags("Posts");

        group.MapPost("/newpost", async (
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
                    throw new BadRequestException("Error No post found", ex.Message);
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

        return group;
    }
}