namespace Thavyra.Rest.Features.Users.Avatar.Get;

public class Request : UserRequest
{
    /// <summary>
    /// This parameter does nothing and will be removed.
    /// </summary>
    public int? Size { get; set; }
}