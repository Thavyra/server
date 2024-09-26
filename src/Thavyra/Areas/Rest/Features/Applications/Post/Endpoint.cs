using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.User;
using Thavyra.Rest.Json;
using Thavyra.Rest.Security;
using Thavyra.Rest.Security.Resource;

namespace Thavyra.Rest.Features.Applications.Post;

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IRequestClient<User_GetById> _userClient;
    private readonly IAuthorizationService _authorizationService;
    private readonly IRequestClient<Application_Create> _applicationClient;

    public Endpoint(
        IRequestClient<User_GetById> userClient, 
        IAuthorizationService authorizationService, 
        IRequestClient<Application_Create> applicationClient)
    {
        _userClient = userClient;
        _authorizationService = authorizationService;
        _applicationClient = applicationClient;
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
            ConsentType = req.ConsentType.HasValue ? req.ConsentType.Value : OpenIddictConstants.ConsentTypes.Explicit,
            Description = req.Description.HasValue ? req.Description.Value : null,
        };
        
        var authorizationResult = await _authorizationService.AuthorizeAsync(User,
            createRequest, Security.Policies.Operation.Application.Create);

        if (authorizationResult.Failure?.FailureReasons is { } reasons)
            foreach (var reason in reasons)
            {
                AddError(reason.Message);
            }

        if (authorizationResult.Failed())
        {
            await SendErrorsAsync(StatusCodes.Status403Forbidden, ct);
            return;
        }

        var applicationResponse = await _applicationClient.GetResponse<ApplicationCreated>(createRequest, ct);

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
            ConsentType = application.ConsentType,
            
            CreatedAt = application.CreatedAt
        }, cancellation: ct);
    }
}