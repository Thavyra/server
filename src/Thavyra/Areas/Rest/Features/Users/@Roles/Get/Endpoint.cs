using FastEndpoints;
using FastEndpoints.Swagger;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Role;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Users.Roles.Get;

public class Endpoint : Endpoint<UserRequest, List<Response>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Role_GetByUser> _getRoles;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Role_GetByUser> getRoles)
    {
        _authorizationService = authorizationService;
        _getRoles = getRoles;
    }

    public override void Configure()
    {
        Get("/users/{User}/roles");
        Summary(x =>
        {
            x.Summary = "Get User Roles";
        });
        Description(x => x.AutoTagOverride("Roles"));
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException();
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user, Security.Policies.Operation.User.ReadRoles);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var response = await _getRoles.GetResponse<Multiple<Role>>(new Role_GetByUser
        {
            UserId = user.Id
        }, ct);

        await SendAsync(response.Message.Items.Select(x => new Response
        {
            Id = x.Id,
            Name = x.Name,
            DisplayName = x.DisplayName
        }).ToList(), cancellation: ct);
    }
}