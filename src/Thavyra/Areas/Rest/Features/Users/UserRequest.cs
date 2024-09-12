using Thavyra.Contracts.User;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Users;

public class UserRequest : RequestWithAuthentication
{
    /// <summary>
    /// User slug retrieved from request fields.
    /// </summary>
    public string? User { get; set; }
}