using FastEndpoints;

namespace Thavyra.Rest.Features.Applications.Icon.Delete;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Delete Icon";
        Description = "Delete the application's custom icon, if set.";
        
        Response(204, "Success");
        Response(404, "Application Not Found");
    }
}