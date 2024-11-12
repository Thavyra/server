using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.Post;

public class Request : RequestWithAuthentication
{
    public JsonOptional<Guid> OwnerId { get; set; }
    
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public JsonOptional<string> Description { get; set; }
}