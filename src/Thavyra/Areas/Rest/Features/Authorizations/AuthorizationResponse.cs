using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Authorizations;

public class AuthorizationResponse
{
    public required Guid Id { get; set; }
    public JsonOptional<Guid> ApplicationId { get; set; }
    public JsonOptional<Guid> UserId { get; set; }

    public required JsonNullable<string> Type { get; set; }
    public required JsonNullable<string> Status { get; set; }

    public required DateTime CreatedAt { get; set; }
}