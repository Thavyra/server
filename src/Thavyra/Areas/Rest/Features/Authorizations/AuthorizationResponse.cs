using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Authorizations;

public class AuthorizationResponse
{
    /// <summary>
    /// id of the connection.
    /// </summary>
    public required Guid Id { get; set; }
    /// <summary>
    /// id of the authorised client.
    /// </summary>
    public JsonOptional<Guid> ApplicationId { get; set; }
    /// <summary>
    /// id of the subject of the connection.
    /// </summary>
    public JsonOptional<Guid> UserId { get; set; }
    /// <summary>
    /// Type of the connection; `permanent` connections will allow implicit authorisation in future requests, `ad-hoc` connections will require continued consent.
    /// </summary>
    public required JsonNullable<string> Type { get; set; }
    /// <summary>
    /// Status of the connection, usually `valid` as revoked connections are not listed.
    /// </summary>
    public required JsonNullable<string> Status { get; set; }
    /// <summary>
    /// When the connection was authorised.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}