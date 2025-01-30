using FastEndpoints;

namespace Thavyra.Rest.Features.Users.Roles.Delete;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Revoke Role";
        Description = "Revoke the role from the user, if they are a member.";
        
        Response(204, "Success");
        Response(404, "User or Role Not Found");
    }
}