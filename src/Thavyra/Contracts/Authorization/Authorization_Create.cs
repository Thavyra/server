namespace Thavyra.Contracts.Authorization;

public record Authorization_Create
{
    public required Guid Id { get; init; }
    public required Guid? ApplicationId { get; init; }
    public required Guid? UserId { get; init; }

    public required string? Type { get; init; }
    public required string? Status { get; init; }
    
    public required List<string> Scopes { get; init; }

    public required DateTime CreatedAt { get; init; }
}