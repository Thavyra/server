using MassTransit;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource.Score;

/// <summary>
/// Authorizes a create operation if the score will belong to the current subject and client.
/// </summary>
public class PrincipalCanCreate : AuthorizationHandler<CreateScoreRequirement, Score_Create>
{
    private readonly IRequestClient<Objective_GetById> _getObjective;

    public PrincipalCanCreate(IRequestClient<Objective_GetById> getObjective)
    {
        _getObjective = getObjective;
    }

    protected override async Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Score_Create resource)
    {
        var response = await _getObjective.GetResponse<Contracts.Scoreboard.Objective>(new Objective_GetById
        {
            Id = resource.ObjectiveId
        });

        return state.AllowPrincipal(resource.UserId, response.Message.ApplicationId);
    }
}