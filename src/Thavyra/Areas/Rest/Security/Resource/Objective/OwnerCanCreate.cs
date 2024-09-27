using MassTransit;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource.Objective;

/// <summary>
/// Authorizes a create operation if the user is the application owner.
/// </summary>
public class OwnerCanCreate : AuthorizationHandler<CreateObjectiveRequirement, Objective_Create>
{
    private readonly IRequestClient<Application_GetById> _getApplication;

    public OwnerCanCreate(IRequestClient<Application_GetById> getApplication)
    {
        _getApplication = getApplication;
    }

    protected override async Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Objective_Create resource)
    {
        var response = await _getApplication.GetResponse<Contracts.Application.Application>(new Application_GetById
        {
            Id = resource.ApplicationId
        });

        return state
            .AllowUser(response.Message.OwnerId)
            .RequireScope(Constants.Scopes.Applications.All);
    }
}