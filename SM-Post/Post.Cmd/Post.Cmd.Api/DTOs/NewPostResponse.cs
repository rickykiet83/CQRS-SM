using Post.Common.DTOs;

namespace Post.Cmd.Api.DTOs;

public class NewPostResponse : BaseResponse
{
    public NewPostResponse()
    {
    }

    public NewPostResponse(Guid id, string message)
    {
        Id = id;
        Message = message;
    }

    public Guid Id { get; set; }   
}