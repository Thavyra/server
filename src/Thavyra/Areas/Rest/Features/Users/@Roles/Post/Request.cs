using FastEndpoints;

namespace Thavyra.Rest.Features.Users.Roles.Post;

public class Request : UserRequest
{
    /// <summary>
    /// id of the role.
    /// </summary>
    public Guid Id { get; set; }
}