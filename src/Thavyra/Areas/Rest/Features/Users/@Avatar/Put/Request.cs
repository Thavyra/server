namespace Thavyra.Rest.Features.Users.Avatar.Put;

public class Request : UserRequest
{
    /// <summary>
    /// Image file to set as avatar.
    /// </summary>
    public IFormFile Avatar { get; set; } = null!;
}