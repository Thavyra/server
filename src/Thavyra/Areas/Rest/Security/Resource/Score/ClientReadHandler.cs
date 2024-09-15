using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource.Score;

/// <summary>
/// Authorizes a read operation if the objective belongs to the current client.
/// </summary>
public class ClientReadHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contracts.Scoreboard.Score>
{
    private readonly IRequestClient<Objective_GetById> _client;

    public ClientReadHandler(IRequestClient<Objective_GetById> client)
    {
        _client = client;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Contracts.Scoreboard.Score resource)
    {
        if (requirement.Name != Operations.Read.Name)
        {
            return;
        }

        var response = await _client.GetResponse<Contracts.Scoreboard.Objective>(new Objective_GetById
        {
            Id = resource.ObjectiveId
        });

        if (context.User.GetClaim("application_id") == response.Message.ApplicationId.ToString())
        {
            context.Succeed(requirement);
        }
    }
}