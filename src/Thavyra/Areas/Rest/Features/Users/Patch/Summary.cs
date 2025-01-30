using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Users.Patch;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Update User";
        Description = "Change a user's username and profile description. Returns the updated user object.";

        var example = Example.User();

        ExampleRequest = new Request
        {
            Username = example.Username,
            Description = example.Description
        };
        
        Response(example: example);
        Response(404, "User Not Found");
    }
}