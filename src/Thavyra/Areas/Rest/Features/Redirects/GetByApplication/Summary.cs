using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Redirects.GetByApplication;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get Application Redirects";
        Description = "Returns the redirect URIs of the application.";

        Response(example: new[] { Example.Redirect() });
        Response(404, "Application Not Found");
    }
}