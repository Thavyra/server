namespace Thavyra.Contracts.Permission;

public record Permission
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
}