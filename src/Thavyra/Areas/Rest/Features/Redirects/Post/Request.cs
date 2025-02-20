using Thavyra.Rest.Documentation;
using Thavyra.Rest.Features.Applications;

namespace Thavyra.Rest.Features.Redirects.Post;

[SchemaName("CreateRedirectRequest")]
public class Request : ApplicationRequest
{
    public string Uri { get; set; } = null!;
}