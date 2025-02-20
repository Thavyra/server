using Thavyra.Rest.Documentation;
using Thavyra.Rest.Features.Users;

namespace Thavyra.Rest.Features.Logins.PutPassword;

[SchemaName("ChangePasswordRequest")]
public class Request : UserRequest
{
    public string? CurrentPassword { get; set; }
    public string Password { get; set; } = null!;
}