using Thavyra.Contracts.User;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Users;

public class UserRequest : RequestWithAuthentication
{
    /// <summary>
    /// User id, `@me` to reference the current subject, or `@&lt;username&gt;` to find by username.
    /// </summary>
    public UserQuery? User { get; set; }
}