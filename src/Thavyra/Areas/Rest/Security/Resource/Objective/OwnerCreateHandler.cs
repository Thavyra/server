using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource.Objective;

/// <summary>
/// Authorizes a create operation if the user is the application owner.
/// </summary>
public class OwnerCreateHandler : AuthorizationHandler<OperationAuthorizationRequirement, Objective_Create>
{
    private readonly IRequestClient<Application_GetById> _client;

    public OwnerCreateHandler(IRequestClient<Application_GetById> client)
    {
        _client = client;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Objective_Create resource)
    {
        if (requirement.Name != Operations.Create.Name)
        {
            return;
        }

        var response = await _client.GetResponse<Contracts.Application.Application>(new Application_GetById
        {
            Id = resource.ApplicationId
        });

        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == response.Message.OwnerId.ToString())
        {
            context.Succeed(requirement);
        }
    }
}