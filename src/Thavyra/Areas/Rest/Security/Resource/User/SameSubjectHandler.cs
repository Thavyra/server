using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security.Resource.User;

/// <summary>
/// Authorizes any operation on a user if they are the current subject.
/// </summary>
public class SameSubjectHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contracts.User.User>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Contracts.User.User resource)
    {
        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.Id.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}