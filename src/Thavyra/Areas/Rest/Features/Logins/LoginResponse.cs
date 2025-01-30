using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Logins;

public class LoginResponse
{
    /// <summary>
    /// id of the login.
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Type of the login: `password` for password logins, or the name of the external provider e.g. `discord`.
    /// </summary>
    public required string Type { get; set; }

    /// <summary>
    /// The account username provided by an external login.
    /// </summary>
    public JsonOptional<string> ProviderUsername { get; set; }
    
    /// <summary>
    /// The account avatar URL provided by an external login.
    /// </summary>
    public JsonOptional<string> ProviderAvatarUrl { get; set; }
    
    /// <summary>
    /// When the login was last successfully used.
    /// </summary>
    public required DateTime UsedAt { get; set; }
    
    /// <summary>
    /// If the login is a password, when it was last changed.
    /// </summary>
    public JsonOptional<DateTime> ChangedAt { get; set; }
}