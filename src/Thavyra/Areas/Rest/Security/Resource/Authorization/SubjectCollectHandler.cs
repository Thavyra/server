using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Authorization;

namespace Thavyra.Rest.Security.Resource.Authorization;

public class SubjectCollectHandler : AuthorizationHandler<OperationAuthorizationRequirement, Authorization_GetByUser>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Authorization_GetByUser resource)
    {
        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.UserId.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}