using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security.Resource.Application;

/// <summary>
/// Authorizes any operation on an application if the user is its owner.
/// </summary>
public class OwnerHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contracts.Application.Application>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Contracts.Application.Application resource)
    {
        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.OwnerId.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}