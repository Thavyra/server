using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Applications.GetByUser;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get User Applications";
        Description = "Returns the applications belonging to a user.";

        Response(example: new[] { Example.Application(), Example.Application() });
        Response(404, "User Not Found");
    }
}