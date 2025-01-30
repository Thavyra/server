using FastEndpoints;

namespace Thavyra.Rest.Features.Authorizations.Delete;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Revoke Connection";
        Description = "Revoke an OpenID connection.";
        
        Response(204, "Success");
        Response(404, "Connection Not Found");
    }
}