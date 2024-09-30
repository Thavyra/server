using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Role;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Users.Roles.Post;

public class Endpoint : Endpoint<Request>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<User_GrantRole> _grantRole;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<User_GrantRole> grantRole)
    {
        _authorizationService = authorizationService;
        _grantRole = grantRole;
    }

    public override void Configure()
    {
        Post("/users/{User}/roles/{Id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = ProcessorState<AuthenticationState>().User;

        if (user is null)
        {
            AddError(x => x.User, "User not found.");
            await SendErrorsAsync(404, ct);
            
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.Role.Grant);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }
        
        Response response = await _grantRole.GetResponse<Success, NotFound>(new User_GrantRole
        {
            UserId = user.Id,
            RoleId = req.Id
        }, ct);

        switch (response)
        {
            case (_, NotFound):
                AddError(x => x.Id, "Role not found.");
                await SendErrorsAsync(404, ct);
                
                return;
            
            case (_, Success):
                await SendOkAsync(ct);
                break;
        }
    }
}