using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Authorizations.Get;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get Connection";
        Description = "Returns the connection object for the given id.";
        
        Response(example: Example.Authorization());
        Response(404, "Connection Not Found");
    }
}