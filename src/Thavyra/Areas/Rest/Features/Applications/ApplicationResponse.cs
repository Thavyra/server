using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Applications;

public class ApplicationResponse
{
    public required string Id { get; set; }
    public required string OwnerId { get; set; }
    public required string Name { get; set; }
    public required JsonNullable<string> Description { get; set; }
    
    public JsonOptional<bool> IsConfidential { get; set; }
    public JsonOptional<string> ClientId { get; set; }
    public JsonOptional<string> ConsentType { get; set; }
    
    public required DateTime CreatedAt { get; set; }
}