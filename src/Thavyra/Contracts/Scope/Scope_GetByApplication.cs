namespace Thavyra.Contracts.Scope;

public record Scope_GetByApplication
{
    public required Guid ApplicationId { get; init; }
}
