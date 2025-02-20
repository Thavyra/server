using FastEndpoints;

namespace Thavyra.Rest.Features.Applications.Icon.Get;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Download Icon";
        Description = "Returns the image file for the application's icon. All icons are in png format with a size of 500x500 pixels.";
        
        Response(200, "Success");
        Response(404, "Application Not Found");
    }
}