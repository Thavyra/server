using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource.Objective;

/// <summary>
/// Authorizes a collect operation if the user is the application owner.
/// </summary>
public class OwnerCollectHandler : AuthorizationHandler<OperationAuthorizationRequirement, Objective_GetByApplication>
{
    private readonly IRequestClient<Application_GetById> _client;

    public OwnerCollectHandler(IRequestClient<Application_GetById> client)
    {
        _client = client;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Objective_GetByApplication resource)
    {
        if (requirement.Name != Operations.Read.Name)
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