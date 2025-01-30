using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Applications;

public class ApplicationResponse
{
    /// <summary>
    /// id of the application.
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// id of the user who owns the application.
    /// </summary>
    public required Guid OwnerId { get; set; }
    
    /// <summary>
    /// Name of the application.
    /// </summary>
    public required string Name { get; set; }
    
    /// <summary>
    /// Description of the application. Can be null.
    /// </summary>
    public required JsonNullable<string> Description { get; set; }
    
    /// <summary>
    /// Whether the application requires a client secret for authentication. Requires the `applications` scope.
    /// </summary>
    public JsonOptional<bool> IsConfidential { get; set; }
    
    /// <summary>
    /// The OpenID Connect client id of the application. Requires the `applications` scope.
    /// </summary>
    public JsonOptional<string> ClientId { get; set; }
    
    /// <summary>
    /// When the application was created.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}