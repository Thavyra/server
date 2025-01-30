using FastEndpoints;

namespace Thavyra.Rest.Features.Users.Avatar.Put;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Upload Avatar";
        Description = "Upload a custom avatar for the user.";
        
        Response(201, "Success");
        Response(404, "User Not Found");
    }
}