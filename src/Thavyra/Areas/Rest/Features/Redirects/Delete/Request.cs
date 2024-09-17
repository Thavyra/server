using Thavyra.Rest.Features.Applications;

namespace Thavyra.Rest.Features.Redirects.Delete;

public class Request : ApplicationRequest
{
    public Guid Id { get; set; }
}