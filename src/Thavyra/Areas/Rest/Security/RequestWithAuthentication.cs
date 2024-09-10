using System.Diagnostics.CodeAnalysis;
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
    /// User ID retrieved from request fields.
    /// </summary>
    public string? UserId { get; set; }
    
    /// <summary>
    /// Application ID retrieved from request fields.
    /// </summary>
    public string? ApplicationId { get; set; }

    /// <summary>
    /// Attempts to parse a user ID from the request.
    /// </summary>
    /// <param name="userId">Contains the parsed value if the method returns true, <see cref="Guid.Empty"/> otherwise.</param>
    /// <returns></returns>
    public bool TryGetUserId(out Guid userId)
    {
        userId = UserId switch
        {
            "@me" or null when Guid.TryParse(Subject, out var subject) => subject,
            not "@me" and not null when Guid.TryParse(UserId, out var id) => id,
            _ => Guid.Empty
        };
        
        return userId != Guid.Empty;
    }

    /// <summary>
    /// Attempts to parse an application ID from the request.
    /// </summary>
    /// <param name="applicationId">Contains the parsed value if the method returns true, <see cref="Guid.Empty"/> otherwise.</param>
    /// <returns></returns>
    public bool TryGetApplicationId(out Guid? applicationId)
    {
        applicationId = ApplicationId switch
        {
            "@me" or null when Guid.TryParse(Client, out var client) => client,
            not "@me" and not null when Guid.TryParse(ApplicationId, out var id) => id,
            _ => Guid.Empty
        };
        
        return applicationId != Guid.Empty;
    }
}