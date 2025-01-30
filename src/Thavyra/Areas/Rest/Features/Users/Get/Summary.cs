using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Users.Get;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get User";
        Description = "Returns the user object for the given id.";
        
        Response(example: Example.User());
        Response(404, "User Not Found");
    }
}