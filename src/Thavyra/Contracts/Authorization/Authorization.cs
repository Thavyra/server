namespace Thavyra.Contracts.Authorization;

public record Authorization
{
    public required Guid Id { get; init; }
    public required Guid? ApplicationId { get; init; }
    public required Guid? Subject { get; init; }

    public required string? Type { get; init; }
    public required string? Status { get; init; }

    public required IReadOnlyList<string> Scopes { get; init; }

    public required DateTime CreatedAt { get; init; }
}