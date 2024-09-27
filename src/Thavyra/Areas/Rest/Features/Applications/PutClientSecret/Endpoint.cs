using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts.Application;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.PutClientSecret;

public class Endpoint : Endpoint<ApplicationRequest, Response>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Application_ResetClientSecret> _resetClientSecret;

    public Endpoint(IAuthorizationService authorizationService, IRequestClient<Application_ResetClientSecret> resetClientSecret)
    {
        _authorizationService = authorizationService;
        _resetClientSecret = resetClientSecret;
    }

    public override void Configure()
    {
        Put("/applications/{Application}/client_secret");
    }

    public override async Task HandleAsync(ApplicationRequest req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.Application is not { } application)
        {
            throw new InvalidOperationException();
        }

        var authorizationResult = await _authorizationService.AuthorizeAsync(User, application,
            Security.Policies.Operation.Application.ResetClientSecret);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var response = await _resetClientSecret.GetResponse<ClientSecretCreated>(new Application_ResetClientSecret
        {
            ApplicationId = application.Id
        }, ct);

        await SendAsync(new Response
        {
            ClientSecret = response.Message.ClientSecret
        }, cancellation: ct);
    }
}