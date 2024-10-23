using MassTransit;
using Thavyra.Contracts.Application;

namespace Thavyra.Rest.Security.Resource.Objective;

public class OwnerCanDelete : AuthorizationHandler<DeleteObjectiveRequirement, Contracts.Scoreboard.Objective>
{
    private readonly IRequestClient<Application_GetById> _getApplication;

    public OwnerCanDelete(IRequestClient<Application_GetById> getApplication)
    {
        _getApplication = getApplication;
    }
    
    protected override async Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state, Contracts.Scoreboard.Objective resource)
    {
        var response = await _getApplication.GetResponse<Contracts.Application.Application>(new Application_GetById
        {
            Id = resource.ApplicationId
        });

        return state
            .AllowSubject(response.Message.OwnerId)
            .RequireScope(Constants.Scopes.Applications.All);
    }
}