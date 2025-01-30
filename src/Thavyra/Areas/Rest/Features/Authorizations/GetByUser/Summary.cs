using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Authorizations.GetByUser;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get User Connections";
        Description = "Returns the OpenID connections authorised by a user.";
        
        Response(example: new[] {Example.Authorization()});
        Response(404, "User Not Found");
    }
}