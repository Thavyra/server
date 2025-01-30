using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications;

public class ApplicationRequest : RequestWithAuthentication
{
    /// <summary>
    /// Application id or `@me` to reference the current client.
    /// </summary>
    public ApplicationQuery? Application { get; set; }
}