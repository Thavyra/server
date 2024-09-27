using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts.Application;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Redirects.Post;

public class Endpoint : Endpoint<Request, RedirectResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Redirect_Create> _createRedirect;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Redirect_Create> createRedirect)
    {
        _authorizationService = authorizationService;
        _createRedirect = createRedirect;
    }

    public override void Configure()
    {
        Post("/applications/{Application}/redirects");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.Application is not { } application)
        {
            throw new InvalidOperationException();
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, application, Security.Policies.Operation.Application.Update);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var response = await _createRedirect.GetResponse<Redirect>(new Redirect_Create
        {
            ApplicationId = application.Id,
            Uri = req.Uri
        }, ct);

        await SendCreatedAtAsync<Get.Endpoint>(new { Id = response.Message.Id }, new RedirectResponse
        {
            Id = response.Message.Id,
            ApplicationId = response.Message.ApplicationId,
            Uri = response.Message.Uri,
            CreatedAt = response.Message.CreatedAt
        }, cancellation: ct);
    }
}