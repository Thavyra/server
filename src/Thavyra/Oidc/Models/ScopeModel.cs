namespace Thavyra.Oidc.Models;

public class ScopeModel
{
    public string? Description { get; set; }
    public string? DisplayName { get; set; }
    public required Guid Id { get; set; }
    public string? Name { get; set; }
}