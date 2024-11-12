using Thavyra.Contracts.User;

namespace Thavyra.Rest.Security.Resource;

public class SetPasswordRequirement : IOperationAuthorizationRequirement;

public class SubjectCanSetPassword : FluentAuthorizationHandler<SetPasswordRequirement, User>
{
    public SubjectCanSetPassword()
    {
        Scope(Constants.Scopes.Sudo);
        Subject(x => x.Id);
    }
}
