using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Redirects.Get;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get Redirect";
        Description = "Returns the Redirect object for the given id.";
        Response(example: Example.Redirect());
        Response(404, "Redirect Not Found");
    }
}