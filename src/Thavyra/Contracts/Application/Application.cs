namespace Thavyra.Contracts.Application;

public record Application
{
    public required Guid Id { get; init; }
    public required Guid OwnerId { get; init; }

    public required string ClientId { get; init; }
    public required string ClientType { get; init; }
    public required string ConsentType { get; init; }

    public required string Type { get; init; }
    public required string Name { get; init; }
    public required string? Description { get; init; }

    public required DateTime CreatedAt { get; init; }
}