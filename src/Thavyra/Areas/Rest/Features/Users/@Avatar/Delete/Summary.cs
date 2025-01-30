using FastEndpoints;

namespace Thavyra.Rest.Features.Users.Avatar.Delete;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Delete Avatar";
        Description = "Delete the user's custom avatar, if set.";
        
        Response(204, "Success");
        Response(404, "User Not Found");
    }
}