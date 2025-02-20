using FastEndpoints;

namespace Thavyra.Rest.Features.Users.Delete;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Delete User";
        Description = "Deactivate the user's account.";
        
        Response(204, "Success");
        Response(404, "User Not Found");
    }
}