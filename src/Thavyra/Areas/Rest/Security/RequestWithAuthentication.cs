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
    public Guid Subject { get; set; }
    
    /// <summary>
    /// Client ID retrieved from claims.
    /// </summary>
    [FromClaim(ClaimType = OpenIddictConstants.Claims.ClientId, IsRequired = false)]
    public Guid Client { get; set; }
}