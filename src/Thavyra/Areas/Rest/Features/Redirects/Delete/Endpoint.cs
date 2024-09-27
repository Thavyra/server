using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Redirects.Delete;

public class Endpoint : Endpoint<Request>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Redirect_Delete> _deleteRedirect;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Redirect_Delete> deleteRedirect)
    {
        _authorizationService = authorizationService;
        _deleteRedirect = deleteRedirect;
    }

    public override void Configure()
    {
        Delete("/applications/{Application}/redirects/{Id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.Application is not { } application)
        {
            throw new InvalidOperationException();
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, application,
                Security.Policies.Operation.Application.Update);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var response = await _deleteRedirect.GetResponse<Success>(new Redirect_Delete
        {
            ApplicationId = application.Id,
            Id = req.Id
        }, ct);

        await SendNoContentAsync(ct);
    }
}