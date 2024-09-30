namespace Thavyra.Contracts.Role;

public record Role
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string DisplayName { get; init; }
}