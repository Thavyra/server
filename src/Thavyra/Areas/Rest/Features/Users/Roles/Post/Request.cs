using FastEndpoints;

namespace Thavyra.Rest.Features.Users.Roles.Post;

public class Request : UserRequest
{
    public Guid Id { get; set; }
}