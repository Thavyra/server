using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Applications.Get;

public class Endpoint : Endpoint<ApplicationRequest, ApplicationResponse>
{
    private readonly IAuthorizationService _authorizationService;

    public Endpoint(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public override void Configure()
    {
        Get("/applications/{Application}");
        AllowAnonymous();
        
        Summary(x =>
        {
            x.Summary = "Get Application";
        });
    }

    public override async Task HandleAsync(ApplicationRequest req, CancellationToken ct)
    {
        var state = ProcessorState<AuthenticationState>();

        if (state.Application is not { } application)
        {
            throw new InvalidOperationException("Could not retrieve application.");
        }

        var authorizationResult =
            await _authorizationService.AuthorizeAsync(User, application, Security.Policies.Operation.Application.Read);

        bool readDetails = authorizationResult.Succeeded;
        
        await SendAsync(new ApplicationResponse
        {
            Id = application.Id.ToString(),
            OwnerId = application.OwnerId.ToString(),

            Name = application.Name,
            Description = application.Description,

            IsConfidential = application.ClientType switch
            {
                OpenIddictConstants.ClientTypes.Confidential when readDetails => true,
                _ when readDetails => false,
                _ => default
            },
            ClientId = readDetails ? application.ClientId : default(JsonOptional<string>),

            CreatedAt = application.CreatedAt
        }, cancellation: ct);
    }
}