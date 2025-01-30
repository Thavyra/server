using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Redirects;

[SchemaName("Redirect")]
public class RedirectResponse
{
    /// <summary>
    /// id of the redirect.
    /// </summary>
    public required Guid Id { get; set; }
    /// <summary>
    /// id of the application.
    /// </summary>
    public required Guid ApplicationId { get; set; }
    /// <summary>
    /// The redirect URI.
    /// </summary>
    public required string Uri { get; set; }
    /// <summary>
    /// When the redirect was created.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}