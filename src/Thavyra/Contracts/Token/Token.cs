namespace Thavyra.Contracts.Token;

public record Token
{
    public required Guid Id { get; init; }
    public required Guid? ApplicationId { get; init; }
    public required Guid? AuthorizationId { get; init; }
    public required Guid? UserId { get; init; }

    public required string? ReferenceId { get; init; }
    public required string? Type { get; init; }
    public required string? Status { get; init; }
    public required string? Payload { get; init; }

    public required DateTime CreatedAt { get; init; }
    public required DateTime? RedeemedAt { get; init; }
    public required DateTime? ExpiresAt { get; init; }
}