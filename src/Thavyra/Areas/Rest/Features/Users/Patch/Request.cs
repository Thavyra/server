using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Users.Patch;

public class Request : RequestWithAuthentication
{
    public JsonOptional<string> Username { get; set; }
    public JsonOptional<string?> Description { get; set; }
}