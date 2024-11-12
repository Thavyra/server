namespace Thavyra.Oidc.Models.Internal;

public class ApplicationModel
{
    /// <summary>
    /// Type of the application (web/native).
    /// </summary>
    public string? ApplicationType { get; set; }

    /// <summary>
    /// OIDC Client ID of the application.
    /// </summary>
    public string? ClientId { get; set; }
    
    /// <summary>
    /// Client type of the application (confidential/public).
    /// </summary>
    public string? ClientType { get; set; }

    /// <summary>
    /// Display name of the application (referred to only as "Name" in other contexts.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Internal ID of the application.
    /// </summary>
    public required Guid Id { get; set; }
}