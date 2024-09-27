namespace Thavyra.Rest.Security.Resource.Objective;

/// <summary>
/// Authorizes a read operation if the objective belongs to the current client.
/// </summary>
public class ClientCanRead : AuthorizationHandler<ReadObjectiveRequirement, Contracts.Scoreboard.Objective>
{
    protected override Task<AuthorizationHandlerState> HandleAsync(AuthorizationHandlerState state,
        Contracts.Scoreboard.Objective resource) => Task.FromResult(state

        .AllowClient(resource.ApplicationId));
}