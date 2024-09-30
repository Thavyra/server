namespace Thavyra.Rest.Features.Applications.Permissions.Put;

public class Response
{
    public required List<PermissionResponse> Granted { get; set; }
    public required List<PermissionResponse> Denied { get; set; }
}