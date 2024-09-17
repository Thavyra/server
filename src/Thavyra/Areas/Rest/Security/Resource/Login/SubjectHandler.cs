using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Login;

namespace Thavyra.Rest.Security.Resource.Login;

public class SubjectHandler : AuthorizationHandler<OperationAuthorizationRequirement, PasswordLogin>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        PasswordLogin resource)
    {
        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == resource.UserId.ToString())
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}