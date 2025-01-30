using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Scopes;

[SchemaName("Scope")]
public class ScopeResponse
{
    /// <summary>
    /// id of the scope.
    /// </summary>
    public required Guid Id { get; set; }
    /// <summary>
    /// Name of the scope.
    /// </summary>
    public required string Name { get; set; }
    /// <summary>
    /// Display name of the scope.
    /// </summary>
    public required string DisplayName { get; set; }
    /// <summary>
    /// Description of the scope (shown on the dashboard and authorisation consent form).
    /// </summary>
    public required string Description { get; set; }
}