using FastEndpoints;

namespace Thavyra.Rest.Features.Scopes.GetByAuthorization;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get Connection Scopes";
        Description = "Returns the scopes authorised for the connection.";
        
        Response(example: new ScopeResponse
        {
            Id = Guid.NewGuid(),
            Name = Constants.Scopes.Account.ReadProfile,
            DisplayName = "View Profile",
            Description = "View your profile and avatar."
        });
        Response(404, "Connection Not Found");
    }
}