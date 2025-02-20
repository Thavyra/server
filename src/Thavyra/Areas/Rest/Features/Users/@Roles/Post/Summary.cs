using FastEndpoints;

namespace Thavyra.Rest.Features.Users.Roles.Post;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Grant Role";
        Description = "Grant a role to a user.";
        
        Response(200, "Success");
        Response(404, "User or Role Not Found");
    }
}