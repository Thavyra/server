using FastEndpoints;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.User;
using Thavyra.Rest.Security;
using Thavyra.Rest.Security.Resource;

namespace Thavyra.Rest.Features.Applications.Post;

public class Endpoint : Endpoint<Request, ApplicationResponse>
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
        Post("/");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        if (!Guid.TryParse(User.GetClaim(OpenIddictConstants.Claims.Subject), out var subject))
        {
            throw new InvalidOperationException();
        }

        var ownerId = req.OwnerId.HasValue ? req.OwnerId.Value : subject;

        var owner = await _userClient.GetResponse<User>(new User_GetById
        {
            Id = ownerId
        }, ct);

        var authorizationResult = await _authorizationService.AuthorizeAsync(User,
            Create<Application>.For(owner.Message), Security.Policies.Operation.Application.Create);

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

        var applicationResponse = await _applicationClient.GetResponse<Application>(new Application_Create
        {
            OwnerId = ownerId,
            Name = req.Name,
            Type = req.Type,
            ConsentType = req.ConsentType.HasValue ? req.ConsentType.Value : OpenIddictConstants.ConsentTypes.Explicit,
            Description = req.Description.HasValue ? req.Description.Value : null,
        }, ct);

        var application = applicationResponse.Message;

        await SendCreatedAtAsync<Get.Endpoint>(new { ApplicationId = application.Id.ToString() }, new ApplicationResponse
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
            ConsentType = application.ClientType,
            
            CreatedAt = application.CreatedAt
        }, cancellation: ct);
    }
}