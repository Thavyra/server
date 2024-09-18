using FastEndpoints;
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
    }

    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        var state = ProcessorState<RequestState>();

        if (state.User is not { } user)
        {
            throw new InvalidOperationException();
        }

        var collectRequest = new Authorization_GetByUser
        {
            UserId = user.Id
        };

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, collectRequest,
                Security.Policies.Operation.Authorization.Read);

        if (authorizationResult.Failed())
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var response = await _getAuthorizations.GetResponse<Multiple<Authorization>>(collectRequest, ct);

        await SendAsync(response.Message.Items.Select(x => new AuthorizationResponse
        {
            Id = x.Id,
            ApplicationId = x.ApplicationId ?? default(JsonOptional<Guid>),
            UserId = x.UserId ?? default(JsonOptional<Guid>),
            Type = x.Type,
            Status = x.Status,
            CreatedAt = x.CreatedAt
        }).ToList(), cancellation: ct);
    }
}