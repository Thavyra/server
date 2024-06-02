namespace Thavyra.Contracts.Scope;

public record Scope_Update
{
    public required Guid Id { get; init; }

    public Change<string> DisplayName { get; init; }
    public Change<string> Description { get; init; }
}