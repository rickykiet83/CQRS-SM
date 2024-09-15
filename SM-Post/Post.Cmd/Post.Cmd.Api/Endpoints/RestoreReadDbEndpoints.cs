using Post.Common.DTOs;
using Post.Common.Exceptions;

namespace Post.Cmd.Api.Endpoints;

public static class RestoreReadDbEndpoints
{
    public static RouteGroupBuilder MapRestoreReadDbEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes
            .MapGroup("/api/v1/restore")
            .WithParameterValidation()
            .WithTags("Posts");


        group.MapPost("/", async (
                ILogger<RouteGroupBuilder> logger,
                ICommandDispatcher dispatcher) =>
            {
                try
                {
                    await dispatcher.SendAsync(new RestoreReadDbCommand());
                    return Results.Created("",
                        new BaseResponse {Message = "Restore read db request completed successfully"});
                }
                catch (InvalidOperationException ex)
                {
                    logger.Log(LogLevel.Warning, ex, "Error restore read db request");
                    throw new BadRequestException("Error restore read db request", ex.Message);
                }
                catch (Exception ex)
                {
                    logger.Log(LogLevel.Error, ex, "Error restore read db request");
                    throw new InternalServerException("Error restore read db", ex.Message);
                }
            })
            .WithSummary("Restore read db")
            .WithDescription("Restore read db")
            .Produces<BaseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            ;
        
        return group;
    }
}