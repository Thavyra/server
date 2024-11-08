namespace Thavyra.Rest.Features.Users.PutAvatar;

public class Request : UserRequest
{
    public IFormFile Avatar { get; set; } = null!;
}