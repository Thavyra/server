namespace Thavyra.Rest.Features.Applications.Permissions.Put;

public class Request : ApplicationRequest
{
    public List<string> Grant { get; set; } = [];
    public List<string> Deny { get; set; } = [];
}