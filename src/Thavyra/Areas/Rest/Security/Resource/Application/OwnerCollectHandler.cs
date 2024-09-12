using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;

namespace Thavyra.Rest.Security.Resource.Application;

public class OwnerCollectHandler : AuthorizationHandler<OperationAuthorizationRequirement, Collect<Contracts.Application.Application>>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Collect<Contracts.Application.Application> resource)
    {
        if (requirement.Name != Operations.Read.Name)
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