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
    [HideFromDocs]
    public Guid Subject { get; set; }
    
    /// <summary>
    /// Application ID retrieved from claims.
    /// </summary>
    [FromClaim(ClaimType = Constants.Claims.ApplicationId, IsRequired = false)]
    [HideFromDocs]
    public Guid ApplicationId { get; set; }

    /// <summary>
    /// Client ID retrieved from claims.
    /// </summary>
    [FromClaim(ClaimType = OpenIddictConstants.Claims.ClientId, IsRequired = false)]
    [HideFromDocs]
    public string? ClientId { get; set; }
}