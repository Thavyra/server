using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Logins.Get;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get User Logins";
        Description = "Returns the logins created by a user. Requires logins scope.";
        
        Response(example: Example.Logins());
        Response(404, "User Not Found");
    }
}