using FastEndpoints;
using Thavyra.Data.Security;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Applications.Get;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get Application";
        Description = "Returns the application object for the given id.";
        Response(200, "Application", example: Example.Application());
        Response(404, "Application Not Found");
    }
}