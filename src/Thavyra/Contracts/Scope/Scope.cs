namespace Thavyra.Contracts.Scope;

public record Scope
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required string Description { get; init; }
}