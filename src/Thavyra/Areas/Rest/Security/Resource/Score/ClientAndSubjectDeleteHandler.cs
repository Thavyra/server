using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource.Score;

/// <summary>
/// Authorizes a delete operation if the score belongs to the current client and subject.
/// </summary>
public class ClientAndSubjectDeleteHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contracts.Scoreboard.Score>
{
    private readonly IRequestClient<Objective_GetById> _client;

    public ClientAndSubjectDeleteHandler(IRequestClient<Objective_GetById> client)
    {
        _client = client;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Contracts.Scoreboard.Score resource)
    {
        if (requirement.Name != Operations.Delete.Name)
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