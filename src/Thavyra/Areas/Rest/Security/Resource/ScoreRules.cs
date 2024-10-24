using MassTransit;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource;

public class CreateScoreRequirement : IOperationAuthorizationRequirement;
public class ReadScoreRequirement : IOperationAuthorizationRequirement;
public class DeleteScoreRequirement : IOperationAuthorizationRequirement;

public class PrincipalCanCreateScore : FluentAuthorizationHandler<CreateScoreRequirement, Score_Create>
{
    private readonly IRequestClient<Objective_GetById> _getObjective;

    public PrincipalCanCreateScore(IRequestClient<Objective_GetById> getObjective)
    {
        _getObjective = getObjective;
        Subject(x => x.UserId);
        ClientAsync(GetApplicationId);
    }

    private async Task<Guid> GetApplicationId(Score_Create resource, CancellationToken cancellationToken)
    {
        var response = await _getObjective.GetResponse<Objective>(new Objective_GetById
        {
            Id = resource.ObjectiveId
        }, cancellationToken);

        return response.Message.ApplicationId;
    }
}

public class ClientCanReadScore : FluentAuthorizationHandler<ReadScoreRequirement, Score>
{
    private readonly IRequestClient<Objective_GetById> _getObjective;

    public ClientCanReadScore(IRequestClient<Objective_GetById> getObjective)
    {
        _getObjective = getObjective;
        ClientAsync(GetApplicationId);
    }

    private async Task<Guid> GetApplicationId(Score resource, CancellationToken cancellationToken)
    {
        var response = await _getObjective.GetResponse<Objective>(new Objective_GetById
        {
            Id = resource.ObjectiveId
        }, cancellationToken);

        return response.Message.ApplicationId;
    }
}
