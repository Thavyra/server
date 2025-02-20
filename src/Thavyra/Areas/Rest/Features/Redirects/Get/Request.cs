using Thavyra.Rest.Features.Applications;

namespace Thavyra.Rest.Features.Redirects.Get;

public class Request : ApplicationRequest
{
    /// <summary>
    /// Redirect id
    /// </summary>
    public Guid Id { get; set; }
}