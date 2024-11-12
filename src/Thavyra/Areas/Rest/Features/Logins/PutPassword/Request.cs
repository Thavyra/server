using Thavyra.Rest.Features.Users;

namespace Thavyra.Rest.Features.Logins.PutPassword;

public class Request : UserRequest
{
    public string? CurrentPassword { get; set; }
    public string Password { get; set; } = null!;
}