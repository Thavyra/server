using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Applications.Post;

public class Request : UserRequest
{
    public JsonOptional<Guid> OwnerId { get; set; }
    
    public string Name { get; set; } = null!;
    public string Type { get; set; } = null!;
    public JsonOptional<string> ConsentType { get; set; }
    public JsonOptional<string> Description { get; set; }
}