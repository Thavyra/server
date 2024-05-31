namespace Thavyra.Contracts.Application;

public record Application
{
    public required string Id { get; init; }
    public required string OwnerId { get; init; }

    public required string Name { get; init; }
    public required string? Description { get; init; }
    public required bool HasSecret { get; init; }

    public required IReadOnlyList<Redirect> Redirects { get; init; }
}