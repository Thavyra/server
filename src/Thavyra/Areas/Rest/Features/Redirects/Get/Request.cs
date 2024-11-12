using Thavyra.Rest.Features.Applications;

namespace Thavyra.Rest.Features.Redirects.Get;

public class Request : ApplicationRequest
{
    public Guid Id { get; set; }
}