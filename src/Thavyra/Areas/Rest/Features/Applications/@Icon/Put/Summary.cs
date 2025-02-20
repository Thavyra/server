using FastEndpoints;

namespace Thavyra.Rest.Features.Applications.Icon.Put;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Upload Icon";
        Description = "Upload a new custom icon for the application.";
        
        Response(201, "Success");
        Response(404, "Application Not Found");
    }
}