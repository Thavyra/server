using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Rest.Features.Applications;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Redirects.GetByApplication;

public class Endpoint : Endpoint<ApplicationRequest, List<RedirectResponse>>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Redirect_GetByApplication> _getRedirects;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Redirect_GetByApplication> getRedirects)
    {
        _authorizationService = authorizationService;
        _getRedirects = getRedirects;
    }

    public override void Configure()
    {
        Get("/applications/{Application}/redirects");
        
        Summary(x =>
        {
            x.Summary = "Get Application Redirects";
        });
    }

    public override async Task HandleAsync(ApplicationRequest req, CancellationToken ct)
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

        var response = await _getRedirects.GetResponse<Multiple<Redirect>>(new Redirect_GetByApplication
        {
            ApplicationId = application.Id
        }, ct);

        await SendAsync(response.Message.Items.Select(x => new RedirectResponse
        {
            Id = x.Id,
            ApplicationId = x.ApplicationId,
            Uri = x.Uri,
            CreatedAt = x.CreatedAt
        }).ToList(), cancellation: ct);
    }
}