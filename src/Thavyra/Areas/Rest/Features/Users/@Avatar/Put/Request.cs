namespace Thavyra.Rest.Features.Users.Avatar.Put;

public class Request : UserRequest
{
    public IFormFile Avatar { get; set; } = null!;
}