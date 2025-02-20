using FastEndpoints;

namespace Thavyra.Rest.Features.Users.Roles.Get;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get User Roles";
        Description = "Returns the roles of which the user is a member.";
        
        Response(example: new Response
        {
            Id = Guid.NewGuid(),
            Name = Constants.Roles.Admin,
            DisplayName = "Admin"
        });
        Response(404, "User Not Found");
    }
}