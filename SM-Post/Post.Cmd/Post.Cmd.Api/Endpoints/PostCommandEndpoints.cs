using Microsoft.AspNetCore.Mvc;
using Post.Cmd.Api.DTOs;
using Post.Common.DTOs;
using Zip.WebAPI.Exceptions;

namespace Post.Cmd.Api.Endpoints;

public static class PostCommandEndpoints
{
    public static RouteGroupBuilder MapPostCommandEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/api/v1")
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
                        new NewPostResponse(id: command.Id, message: "New post creation request completed successfully"));
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
            .Produces(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            ;

        return group;
    }
}