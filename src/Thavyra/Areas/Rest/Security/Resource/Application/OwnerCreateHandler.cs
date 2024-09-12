using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security.Resource.Application;

/// <summary>
/// Authorizes a create operation if the user will be the owner.
/// </summary>
public class OwnerCreateHandler : AuthorizationHandler<OperationAuthorizationRequirement, Create<Contracts.Application.Application>>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Create<Contracts.Application.Application> resource)
    {
        if (requirement.Name != Operations.Create.Name)
        {
            return Task.CompletedTask;
        }

        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.User.Id.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}