using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications;

public class ApplicationRequest : RequestWithAuthentication
{
    /// <summary>
    /// Application slug retrieved from request fields.
    /// </summary>
    public string? Application { get; set; }
}