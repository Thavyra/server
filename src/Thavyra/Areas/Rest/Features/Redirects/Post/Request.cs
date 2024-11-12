using Thavyra.Rest.Features.Applications;

namespace Thavyra.Rest.Features.Redirects.Post;

public class Request : ApplicationRequest
{
    public string Uri { get; set; } = null!;
}