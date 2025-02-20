using Thavyra.Rest.Documentation;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.Post;

[SchemaName("CreateApplicationRequest")]
public class Request : RequestWithAuthentication
{
    /// <summary>
    /// User to set as owner of the application. Defaults to the current user.
    /// </summary>
    public JsonOptional<Guid> OwnerId { get; set; }
    
    /// <summary>
    /// Name of the application.
    /// </summary>
    public string Name { get; set; } = null!;
    /// <summary>
    /// Use `web` for confidential clients, `native` for public clients.
    /// </summary>
    public string Type { get; set; } = null!;
    
    /// <summary>
    /// Description of the application.
    /// </summary>
    public JsonOptional<string> Description { get; set; }
}