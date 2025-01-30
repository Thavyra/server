namespace Thavyra.Rest.Features.Applications.Permissions.Put;

public class Request : ApplicationRequest
{
    /// <summary>
    /// Permissions to be enabled. Items are ignored if already enabled. 
    /// </summary>
    public List<string> Grant { get; set; } = [];
    
    /// <summary>
    /// Permissions to be disabled. Items are ignored if already disabled.
    /// </summary>
    public List<string> Deny { get; set; } = [];
}