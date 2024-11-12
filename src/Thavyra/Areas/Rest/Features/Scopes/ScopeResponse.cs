namespace Thavyra.Rest.Features.Scopes;

public class ScopeResponse
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
    public required string Description { get; set; }
}