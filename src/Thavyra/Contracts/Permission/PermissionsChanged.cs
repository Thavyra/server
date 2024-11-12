namespace Thavyra.Contracts.Permission;

public record PermissionsChanged
{
    public required Guid ApplicationId { get; init; }
    public required List<Permission> Granted { get; init; }
    public required List<Permission> Denied { get; init; }
    public required List<Permission> CurrentPermissions { get; init; }
}