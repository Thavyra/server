using FastEndpoints;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Features.Applications.Permissions.Get;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get Permissions";
        Description = "Returns the permissions enabled by the application.";

        Response(example: new[]
        {
            OpenIddictConstants.Permissions.Endpoints.Authorization,
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.ResponseTypes.Code,
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials
        });
        Response(404, "Application Not Found");
    }
}