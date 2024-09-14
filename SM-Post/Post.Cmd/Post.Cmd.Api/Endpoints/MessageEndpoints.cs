using CQRS.Core.Exceptions;
using Post.Common.DTOs;
using Post.Common.Exceptions;

namespace Post.Cmd.Api.Endpoints;

public static class MessageEndpoints
{
    public static RouteGroupBuilder MapMessageEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/api/v1/messages")
            .WithParameterValidation()
            .WithTags("Messages");

        group.MapPut("/{id:guid}", async (Guid id, EditMessageCommand command,
                ILogger<RouteGroupBuilder> logger,
                ICommandDispatcher commandDispatcher) =>
            {
                try
                {
                    command.Id = id;
                    await commandDispatcher.SendAsync(command);
                    return Results.Ok(new BaseResponse
                    {
                        Message = "Edit message request completed successfully"
                    });
                }
                catch (InvalidOperationException ex)
                {
                    logger.Log(LogLevel.Warning, ex, "Error edit message");
                    throw new BadRequestException("Error edit message", ex.Message);
                }
                catch (AggregateNotFoundException ex)
                {
                    logger.Log(LogLevel.Error, ex, "Could not retrieve aggregate, client passed an invalid post ID targetting the aggregate!");
                    throw new NotFoundException(ex.Message);
                }
                catch (Exception ex)
                {
                    const string message = "Error while processing request to edit the message of a post";
                    logger.Log(LogLevel.Error, ex, message);
                    throw new InternalServerException(ex.Message);
                }
            }).WithSummary("Edit a message")
            .WithDescription("Edit a message with the specified details")
            .Produces<BaseResponse>()
            .ProducesProblem(StatusCodes.Status404NotFound)
            ;

        return group;
    }
}