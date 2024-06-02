namespace Thavyra.Contracts.Scope;

public record Scope_Create
{
    public Guid? Id { get; init; }

    public required string Name { get; init; }
    public required string DisplayName { get; init; }
    public required string Description { get; init; }
}