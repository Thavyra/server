using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Applications.Post;

public class Request
{
    public JsonOptional<Guid> OwnerId { get; set; }
    
    public required string Name { get; set; }
    public required string Type { get; set; }
    public JsonOptional<string> ConsentType { get; set; }
    public JsonOptional<string> Description { get; set; }
}