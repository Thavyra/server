using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Applications.Permissions;

[SchemaName("Permission")]
public class PermissionResponse
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string DisplayName { get; set; }
}