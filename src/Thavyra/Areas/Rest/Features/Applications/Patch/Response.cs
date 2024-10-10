using Thavyra.Rest.Features.Applications.Permissions;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Applications.Patch;

public class Response : ApplicationResponse
{
    public JsonOptional<List<PermissionResponse>> Permissions { get; set; }
}