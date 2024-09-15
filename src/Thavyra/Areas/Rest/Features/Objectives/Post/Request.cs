using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Objectives.Post;

public class Request : RequestWithAuthentication
{
    public string Name { get; set; } = null!;
}