using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Users.Patch;

public class Request : UserRequest
{
    /// <summary>
    /// New username, must be unique. Requires `sudo` scope.
    /// </summary>
    public JsonOptional<string> Username { get; set; }
    /// <summary>
    /// New profile description.
    /// </summary>
    public JsonOptional<string?> Description { get; set; }
}