using MassTransit;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource.Score;

/// <summary>
/// Authorizes a read operation if the objective belongs to the current client.
/// </summary>
public class ClientCanRead : AuthorizationHandler<ReadScoreRequirement, Contracts.Scoreboard.Score>
{
    private readonly IRequestClient<Objective_GetById> _getObjective;

    public ClientCanRead(IRequestClient<Objective_GetById> getObjective)
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

        return state
            .AllowClient(response.Message.ApplicationId);
    }
}