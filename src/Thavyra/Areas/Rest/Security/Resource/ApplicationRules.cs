using FluentValidation;
using MassTransit;
using Thavyra.Contracts;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Role;

namespace Thavyra.Rest.Security.Resource;

public class CreateApplicationRequirement : IOperationAuthorizationRequirement;

public class ReadApplicationRequirement : IOperationAuthorizationRequirement;

public class ReadApplicationObjectivesRequirement : IOperationAuthorizationRequirement;

public class ReadApplicationTransactionsRequirement : IOperationAuthorizationRequirement;

public class UpdateApplicationRequirement : IOperationAuthorizationRequirement;

public class ResetClientSecretRequirement : IOperationAuthorizationRequirement;

public class DeleteApplicationRequirement : IOperationAuthorizationRequirement;

public class
    AdminCanReadApplication : FluentAuthorizationHandler<ReadApplicationRequirement, Application>
{
    public AdminCanReadApplication(IRequestClient<User_HasRole> hasRole)
    {
        Scope(Constants.Scopes.Admin);
        
        RuleFor(x => x.Context.User)
            .MustAsync(async (x, ct) =>
            {
                var response = await hasRole.GetResponse<Correct, Incorrect>(new User_HasRole
                {
                    UserId = x.GetSubject(),
                    RoleName = Constants.Roles.Admin
                }, ct);

                return response.Is(out Response<Correct> _);
            });
    }
}

public class OwnerCanCreateApplication : FluentAuthorizationHandler<CreateApplicationRequirement, Application_Create>
{
    public OwnerCanCreateApplication()
    {
        Scope(Constants.Scopes.Applications.All);
        Subject(x => x.OwnerId);
    }
}

public class
    OwnerCanReadApplication : FluentAuthorizationHandler<ReadApplicationRequirement, Application>
{
    public OwnerCanReadApplication()
    {
        Scope(Constants.Scopes.Applications.Read);
        Subject(x => x.OwnerId);
    }
}

public class OwnerCanReadApplicationObjectives : FluentAuthorizationHandler<ReadApplicationObjectivesRequirement,
    Application>
{
    public OwnerCanReadApplicationObjectives()
    {
        Scope(Constants.Scopes.Applications.Read);
        Subject(x => x.OwnerId);
    }
}

public class OwnerCanReadApplicationTransactions : FluentAuthorizationHandler<ReadApplicationTransactionsRequirement,
    Application>
{
    public OwnerCanReadApplicationTransactions()
    {
        Scope(Constants.Scopes.Applications.Read);
        Subject(x => x.OwnerId);
    }
}

public class
    OwnerCanReadClientSecret : FluentAuthorizationHandler<ResetClientSecretRequirement,
    Application>
{
    public OwnerCanReadClientSecret()
    {
        Scope(Constants.Scopes.Sudo);
        Subject(x => x.OwnerId);
    }
}

public class
    OwnerCanUpdateApplication : FluentAuthorizationHandler<UpdateApplicationRequirement,
    Application>
{
    public OwnerCanUpdateApplication()
    {
        Scope(Constants.Scopes.Applications.All);
        Subject(x => x.OwnerId);
    }
}

public class OwnerCanDeleteApplication : FluentAuthorizationHandler<DeleteApplicationRequirement, Application>
{
    public OwnerCanDeleteApplication()
    {
        Scope(Constants.Scopes.Applications.All);
        Subject(x => x.OwnerId);
    }
}

public class ClientCanReadApplicationObjectives : FluentAuthorizationHandler<ReadApplicationObjectivesRequirement,
    Application>
{
    public ClientCanReadApplicationObjectives()
    {
        Client(x => x.Id);
    }
}

public class SubjectCanReadApplication : FluentAuthorizationHandler<ReadApplicationRequirement, Application>
{
    public SubjectCanReadApplication()
    {
        Scope(Constants.Scopes.Applications.Read);
        Subject(x => x.Id);
    }
}

public class SubjectCanReadApplicationObjectives : FluentAuthorizationHandler<ReadApplicationObjectivesRequirement,
    Application>
{
    public SubjectCanReadApplicationObjectives()
    {
        Scope(Constants.Scopes.Applications.Read);
        Subject(x => x.Id);
    }
}

public class SubjectCanReadApplicationTransactions : FluentAuthorizationHandler<ReadApplicationTransactionsRequirement,
    Application>
{
    public SubjectCanReadApplicationTransactions()
    {
        Scope(Constants.Scopes.Applications.Read);
        Subject(x => x.Id);
    }
}

public class
    SubjectCanResetClientSecret : FluentAuthorizationHandler<ResetClientSecretRequirement,
    Application>
{
    public SubjectCanResetClientSecret()
    {
        Scope(Constants.Scopes.Sudo);
        Subject(x => x.Id);
    }
}

public class
    SubjectCanUpdateApplication : FluentAuthorizationHandler<UpdateApplicationRequirement, Application>
{
    public SubjectCanUpdateApplication()
    {
        Scope(Constants.Scopes.Applications.All);
        Subject(x => x.Id);
    }
}