using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security.Resource.Objective;

/// <summary>
/// Authorizes a read operation if the objective belongs to the current client.
/// </summary>
public class ClientReadHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contracts.Scoreboard.Objective>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Contracts.Scoreboard.Objective resource)
    {
        if (requirement.Name != Operations.Read.Name)
        {
            return Task.CompletedTask;
        }

        if (context.User.GetClaim("application_id") == resource.ApplicationId.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}