using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security.Resource.Authorization;

/// <summary>
/// Authorizes any operation if the subject is the authorization user.
/// </summary>
public class SubjectHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contracts.Authorization.Authorization>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Contracts.Authorization.Authorization resource)
    {
        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.UserId?.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}