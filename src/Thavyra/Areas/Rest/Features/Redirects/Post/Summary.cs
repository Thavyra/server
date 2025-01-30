using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Redirects.Post;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Create Redirect";
        Description = "Add a new redirect URI to the application. Returns a Redirect object on success.";
        ExampleRequest = new
        {
            Uri = "https://example.com/callback"
        };
        Response(example: Example.Redirect());
        Response(404, "Application Not Found");
    }
}