using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Application;

namespace Thavyra.Rest.Security.Resource.Application;

/// <summary>
/// Authorizes a create operation if the user will be the owner.
/// </summary>
public class OwnerCreateHandler : AuthorizationHandler<OperationAuthorizationRequirement, Application_Create>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Application_Create resource)
    {
        if (requirement.Name != Operations.Create.Name)
        {
            return Task.CompletedTask;
        }

        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.OwnerId.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}