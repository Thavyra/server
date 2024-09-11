using FastEndpoints;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security;

/// <summary>
/// A request with subject/client claims set, and/or with UserId/ApplicationId fields.
/// </summary>
public class RequestWithAuthentication
{
    /// <summary>
    /// Subject ID retrieved from claims.
    /// </summary>
    [FromClaim(ClaimType = OpenIddictConstants.Claims.Subject, IsRequired = false)]
    public string? Subject { get; set; }
    
    /// <summary>
    /// Client ID retrieved from claims.
    /// </summary>
    [FromClaim(ClaimType = OpenIddictConstants.Claims.ClientId, IsRequired = false)]
    public string? Client { get; set; }

    /// <summary>
    /// User slug retrieved from request fields.
    /// </summary>
    public string? UserSlug { get; set; }
    
    /// <summary>
    /// Application slug retrieved from request fields.
    /// </summary>
    public string? ApplicationSlug { get; set; }
}