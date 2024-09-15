using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Application;

namespace Thavyra.Rest.Security.Resource.Objective;

/// <summary>
/// Authorizes any operation if the user is the application owner.
/// </summary>
public class OwnerHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contracts.Scoreboard.Objective>
{
    private readonly IRequestClient<Application_GetById> _client;

    public OwnerHandler(IRequestClient<Application_GetById> client)
    {
        _client = client;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Contracts.Scoreboard.Objective resource)
    {
        var response = await _client.GetResponse<Contracts.Application.Application>(new Application_GetById
        {
            Id = resource.ApplicationId
        });

        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == response.Message.Id.ToString())
        {
            context.Succeed(requirement);
        }
    }
}