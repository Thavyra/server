using FastEndpoints;

namespace Thavyra.Rest.Features.Redirects.Delete;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Delete Redirect";
        Description = "Delete a redirect URI from an application.";
        
        Response(204, "Success");
        Response(404, "Application or Redirect Not Found");
    }
}