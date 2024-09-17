using Thavyra.Rest.Features.Users;

namespace Thavyra.Rest.Features.Logins.PutPassword;

public class Request : UserRequest
{
    public string Password { get; set; } = default!;
}