using FastEndpoints;

namespace Thavyra.Rest.Features.Applications.Delete;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Delete Application";
        Description = "Delete an application.";
        
        Response(204, "Success");
        Response(404, "Application Not Found");
    }
}