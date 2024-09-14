using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using OpenIddict.Abstractions;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Transaction;

namespace Thavyra.Rest.Security.Resource.Transaction;

/// <summary>
/// Authorizes a collection read operation if the user is the owner of the application.
/// </summary>
public class OwnerCollectHandler : AuthorizationHandler<OperationAuthorizationRequirement, Transaction_GetByApplication>
{
    private readonly IClientFactory _clientFactory;

    public OwnerCollectHandler(IClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }
    
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement,
        Transaction_GetByApplication resource)
    {
        if (requirement.Name != Operations.Read.Name)
        {
            return;
        }

        var client = _clientFactory.CreateRequestClient<Application_GetById>();
        
        var response = await client.GetResponse<Contracts.Application.Application>(new Application_GetById
        {
            Id = resource.ApplicationId
        });

        if (context.User.GetClaim(OpenIddictConstants.Claims.Subject) == response.Message.OwnerId.ToString())
        {
            context.Succeed(requirement);
        }
    }
}