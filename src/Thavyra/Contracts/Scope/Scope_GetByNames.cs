namespace Thavyra.Contracts.Scope;

public record Scope_GetByNames
{
    public required List<string> Names { get; init; }
}