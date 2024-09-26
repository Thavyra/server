using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource.Score;

/// <summary>
/// Authorizes a create operation if the score will belong to the current subject and client.
/// </summary>
public class ClientAndSubjectCreateHandler : AuthorizationHandler<OperationAuthorizationRequirement, Score_Create>
{
    private readonly IRequestClient<Objective_GetById> _client;

    public ClientAndSubjectCreateHandler(IRequestClient<Objective_GetById> client)
    {
        _client = client;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Score_Create resource)
    {
        if (requirement.Name != Operations.Create.Name)
        {
            return;
        }

        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) != resource.UserId.ToString())
        {
            return;
        }

        var response = await _client.GetResponse<Contracts.Scoreboard.Objective>(new Objective_GetById
        {
            Id = resource.ObjectiveId
        });

        if (context.User.GetClaim(Constants.Claims.ApplicationId) == response.Message.ApplicationId.ToString())
        {
            context.Succeed(requirement);
        }
    }
}