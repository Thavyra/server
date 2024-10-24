namespace Thavyra.Contracts.Authorization;

public record Authorization_Revoke
{
    public required Guid AuthorizationId { get; init; }
}