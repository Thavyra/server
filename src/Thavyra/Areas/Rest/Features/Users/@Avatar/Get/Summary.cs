using FastEndpoints;

namespace Thavyra.Rest.Features.Users.Avatar.Get;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Download Avatar";
        Description = "Returns the image file for the user's avatar.";
        
        Response(200, "Success");
        Response(404, "User Not Found");
    }
}