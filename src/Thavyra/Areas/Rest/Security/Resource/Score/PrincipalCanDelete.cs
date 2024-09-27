using MassTransit;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource.Score;

/// <summary>
/// Authorizes a delete operation if the score belongs to the current client and subject.
/// </summary>
public class PrincipalCanDelete : AuthorizationHandler<DeleteScoreRequirement, Contracts.Scoreboard.Score>
{
    private readonly IRequestClient<Objective_GetById> _getObjective;

    public PrincipalCanDelete(IRequestClient<Objective_GetById> getObjective)
    {
        _getObjective = getObjective;
    }

    protected override async Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Scoreboard.Score resource)
    {
        var response = await _getObjective.GetResponse<Contracts.Scoreboard.Objective>(new Objective_GetById
        {
            Id = resource.ObjectiveId
        });

        return state.AllowPrincipal(resource.UserId, response.Message.ApplicationId);
    }
}