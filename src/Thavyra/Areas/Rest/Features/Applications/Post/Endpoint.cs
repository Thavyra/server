using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Application;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Applications.Post;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Application_Create> _createApplication;

    public Endpoint(IAuthorizationService authorizationService, 
        IRequestClient<Application_Create> createApplication)
    {
        _authorizationService = authorizationService;
        _createApplication = createApplication;
    }

    public override void Configure()
    {
        Post("/applications");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var ownerId = req.OwnerId.HasValue ? req.OwnerId.Value : req.Subject;

        if (ownerId == default)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var createRequest = new Application_Create
        {
            OwnerId = ownerId,
            Name = req.Name,
            Type = req.Type,
            Description = req.Description.HasValue ? req.Description.Value : null,
        };
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User,
            createRequest, Security.Policies.Operation.Application.Create);

        if (!authorizationResult.Succeeded)
        {
            await this.SendAuthorizationFailureAsync(authorizationResult.Failure, ct);
            return;
        }

        var applicationResponse = await _createApplication.GetResponse<ApplicationCreated>(createRequest, ct);

        var application = applicationResponse.Message.Application;

        await SendCreatedAtAsync<Get.Endpoint>(new { Application = application.Id.ToString() }, new Response
        {
            Id = application.Id.ToString(),
            OwnerId = application.OwnerId.ToString(),

            Name = application.Name,
            Description = application.Description,

            IsConfidential = application.ClientType switch
            {
                OpenIddictConstants.ClientTypes.Confidential => true,
                _ => false
            },
            ClientId = application.ClientId,
            ClientSecret = applicationResponse.Message.ClientSecret ?? default(JsonOptional<string>),
            CreatedAt = application.CreatedAt
        }, cancellation: ct);
    }
}