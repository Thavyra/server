using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Application;

namespace Thavyra.Rest.Security.Resource.Transaction;

/// <summary>
/// Authorizes a read operation if the user is the owner of the application.
/// </summary>
public class OwnerReadHandler : AuthorizationHandler<OperationAuthorizationRequirement, Contracts.Transaction.Transaction>
{
    private readonly IRequestClient<Application_GetById> _client;

    public OwnerReadHandler(IRequestClient<Application_GetById> client)
    {
        _client = client;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Contracts.Transaction.Transaction resource)
    {
        if (requirement.Name != Operations.Read.Name)
        {
            return;
        }

        var response = await _client.GetResponse<Contracts.Application.Application>(new Application_GetById
        {
            Id = resource.ApplicationId
        });

        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == response.Message.OwnerId.ToString())
        {
            context.Succeed(requirement);
        }
    }
}