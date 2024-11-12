using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Redirects.Get;

public class Endpoint : Endpoint<Request, RedirectResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Redirect_GetById> _getRedirect;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Redirect_GetById> getRedirect)
    {
        _authorizationService = authorizationService;
        _getRedirect = getRedirect;
    }

    public override void Configure()
    {
        Get("/applications/{Application}/redirects/{Id}");
        
        Summary(x =>
        {
            x.Summary = "Get Redirect";
        });
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.Application is not { } application)
        {
            throw new InvalidOperationException();
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, application, Security.Policies.Operation.Application.Read);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        Response response = await _getRedirect.GetResponse<Redirect, NotFound>(new Redirect_GetById
        {
            ApplicationId = application.Id,
            Id = req.Id
        }, ct);

        switch (response)
        {
            case (_, NotFound):
                await SendNotFoundAsync(ct);
                return;
            
            case (_, Redirect redirect):
                await SendAsync(new RedirectResponse
                {
                    Id = redirect.Id,
                    ApplicationId = redirect.ApplicationId,
                    Uri = redirect.Uri,
                    CreatedAt = redirect.CreatedAt
                }, cancellation: ct);
                break;
        }
    }
}