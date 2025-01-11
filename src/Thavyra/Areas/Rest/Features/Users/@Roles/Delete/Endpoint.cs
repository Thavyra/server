using FastEndpoints;
using FastEndpoints.Swagger;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Role;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Users.Roles.Delete;

public class Endpoint : Endpoint<Request>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<User_DenyRole> _denyRole;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<User_DenyRole> denyRole)
    {
        _authorizationService = authorizationService;
        _denyRole = denyRole;
    }

    public override void Configure()
    {
        Delete("/users/{User}/roles/{Id}");
        Summary(x =>
        {
            x.Summary = "Revoke User Role";
        });
        Description(x => x.AutoTagOverride("Roles"));
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = ProcessorState<AuthenticationState>().User;

        if (user is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.Role.Deny);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        _ = await _denyRole.GetResponse<Success>(new User_DenyRole
        {
            UserId = user.Id,
            RoleId = req.Id
        }, ct);

        await SendNoContentAsync(ct);
    }
}