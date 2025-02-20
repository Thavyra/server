using FastEndpoints;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Features.Applications.Permissions.Put;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Update Permissions";
        Description = """
                      Modify permissions for the application. Returns the resulting set of enabled permissions. 
                      Request will fail if the user or application is not authorised to grant or deny any one of the requested permissions.
                      """;

        ExampleRequest = new Request
        {
            Deny =
            [
                Constants.Permissions.Scopes.Account.ReadProfile,
                Constants.Permissions.Scopes.Transactions.All
            ]
        };
        Response(example: new[]
        {
            OpenIddictConstants.Permissions.Endpoints.Authorization,
            OpenIddictConstants.Permissions.Endpoints.Token,
            OpenIddictConstants.Permissions.ResponseTypes.Code,
            OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
            OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
            OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
            Constants.Permissions.Scopes.Account.ReadProfile, 
            Constants.Permissions.Scopes.Transactions.All
        });
        Response(404, "Application Not Found");
    }
}