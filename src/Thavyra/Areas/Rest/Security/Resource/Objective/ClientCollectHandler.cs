using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource.Objective;

/// <summary>
/// Authorizes a collect operation for the current client.
/// </summary>
public class ClientCollectHandler : AuthorizationHandler<OperationAuthorizationRequirement, Objective_GetByApplication>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Objective_GetByApplication resource)
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