using FastEndpoints;
using FastEndpoints.Swagger;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Authorization;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Authorizations.GetByUser;

public class Endpoint : Endpoint<UserRequest, List<AuthorizationResponse>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Authorization_GetByUser> _getAuthorizations;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Authorization_GetByUser> getAuthorizations)
    {
        _authorizationService = authorizationService;
        _getAuthorizations = getAuthorizations;
    }

    public override void Configure()
    {
        Get("/users/{User}/authorizations");
        
        Description(x => x.AutoTagOverride("Connections"));
        
        Summary(x =>
        {
            x.Summary = "Get User Connections";
        });
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException();
        }
        
        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, user,
                Security.Policies.Operation.User.ReadAuthorizations);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }
        
        var request = new Authorization_GetByUser
        {
            Subject = user.Id
        };

        var response = await _getAuthorizations.GetResponse<Multiple<Authorization>>(request, ct);

        await SendAsync(response.Message.Items.Select(x => new AuthorizationResponse
        {
            Id = x.Id,
            ApplicationId = x.ApplicationId ?? default(JsonOptional<Guid>),
            UserId = x.Subject ?? default(JsonOptional<Guid>),
            Type = x.Type,
            Status = x.Status,
            CreatedAt = x.CreatedAt
        }).ToList(), cancellation: ct);
    }
}