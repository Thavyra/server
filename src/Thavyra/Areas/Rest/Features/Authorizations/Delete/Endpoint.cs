using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Authorization;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Authorizations.Delete;

public class Endpoint : Endpoint<Request>
{
    private readonly IRequestClient<Authorization_GetById> _getAuthorization;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Authorization_Delete> _deleteAuthorization;

    public Endpoint(
        IRequestClient<Authorization_GetById> getAuthorization, 
        IAuthorizationService authorizationService, 
        IRequestClient<Authorization_Delete> deleteAuthorization)
    {
        _getAuthorization = getAuthorization;
        _authorizationService = authorizationService;
        _deleteAuthorization = deleteAuthorization;
    }

    public override void Configure()
    {
        Delete("/authorizations/{Id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _getAuthorization.GetResponse<Authorization, NotFound>(new Authorization_GetById
        {
            Id = req.Id
        }, ct);

        if (!response.Is(out Response<Authorization>? r) || r.Message is not { } authorization)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User,
                authorization, Security.Policies.Operation.Authorization.Delete);

        if (authorizationResult.Failed())
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var deleteResponse = await _deleteAuthorization.GetResponse<Success>(new Authorization_Delete
        {
            Id = authorization.Id
        }, ct);

        await SendNoContentAsync(ct);
    }
}