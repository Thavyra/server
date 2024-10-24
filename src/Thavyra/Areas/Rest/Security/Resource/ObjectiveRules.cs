using MassTransit;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Scoreboard;

namespace Thavyra.Rest.Security.Resource;

public class CreateObjectiveRequirement : IOperationAuthorizationRequirement;
public class ReadObjectiveRequirement : IOperationAuthorizationRequirement;
public class UpdateObjectiveRequirement : IOperationAuthorizationRequirement;
public class DeleteObjectiveRequirement : IOperationAuthorizationRequirement;

public class
    ApplicationOwnerCanCreateObjective : FluentAuthorizationHandler<CreateObjectiveRequirement,
    Objective_Create>
{
    public ApplicationOwnerCanCreateObjective(IRequestClient<Application_GetById> getApplication)
    {
        Scope(Constants.Scopes.Applications.All);
        SubjectAsync(async (objective, ct) =>
        {
            var response = await getApplication.GetResponse<Application>(new Application_GetById
            {
                Id = objective.ApplicationId
            }, ct);

            return response.Message.OwnerId;
        });
    }
}

public class
    ApplicationOwnerCanReadObjective : FluentAuthorizationHandler<ReadObjectiveRequirement,
    Objective>
{
    public ApplicationOwnerCanReadObjective(IRequestClient<Application_GetById> getApplication)
    {
        Scope(Constants.Scopes.Applications.All);
        SubjectAsync(async (objective, ct) =>
        {
            var response = await getApplication.GetResponse<Application>(new Application_GetById
            {
                Id = objective.ApplicationId
            }, ct);

            return response.Message.OwnerId;
        });
    }
}

public class
    ApplicationOwnerCanUpdateObjective : FluentAuthorizationHandler<UpdateObjectiveRequirement, Objective>
{
    public ApplicationOwnerCanUpdateObjective(IRequestClient<Application_GetById> getApplication)
    {
        Scope(Constants.Scopes.Applications.All);
        SubjectAsync(async (objective, ct) =>
        {
            var response = await getApplication.GetResponse<Application>(new Application_GetById
            {
                Id = objective.ApplicationId
            }, ct);

            return response.Message.OwnerId;
        });
    }
}

public class
    ApplicationOwnerCanDeleteObjective : FluentAuthorizationHandler<DeleteObjectiveRequirement, Objective>
{
    public ApplicationOwnerCanDeleteObjective(IRequestClient<Application_GetById> getApplication)
    {
        Scope(Constants.Scopes.Applications.All);
        SubjectAsync(async (objective, ct) =>
        {
            var response = await getApplication.GetResponse<Application>(new Application_GetById
            {
                Id = objective.ApplicationId
            }, ct);

            return response.Message.OwnerId;
        });
    }
}

public class
    ClientCanReadObjective : FluentAuthorizationHandler<ReadObjectiveRequirement, Objective>
{
    public ClientCanReadObjective()
    {
        Client(x => x.ApplicationId);
    }
}
