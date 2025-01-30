using FastEndpoints;
using Thavyra.Data.Security;

namespace Thavyra.Rest.Features.Applications.ClientSecret.Put;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Reset Client Secret";
        Description = "Reset the OpenID Connect client secret of an application. Returns the newly generated secret.";
        
        Response(example: new Response
        {
            ClientSecret = Secret.NewSecret(32).ToString()
        });
        Response(404, "Application Not Found");
    }
}