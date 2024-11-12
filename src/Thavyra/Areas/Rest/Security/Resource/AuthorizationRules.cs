using FluentValidation;
using Thavyra.Contracts.Authorization;

namespace Thavyra.Rest.Security.Resource;

public class ReadAuthorizationRequirement : IOperationAuthorizationRequirement;
public class RevokeAuthorizationRequirement : IOperationAuthorizationRequirement;

public class
    SubjectCanReadAuthorization : FluentAuthorizationHandler<ReadAuthorizationRequirement, Authorization>
{
    public SubjectCanReadAuthorization()
    {
        Scope(Constants.Scopes.Authorizations.Read);
        RuleFor(x => x.Resource.Subject)
            .NotNull()
            .DependentRules(() =>
            {
                Subject(x => x.Subject!.Value);
            });
    }
}

public class
    SubjectCanRevokeAuthorization : FluentAuthorizationHandler<RevokeAuthorizationRequirement, Authorization>
{
    public SubjectCanRevokeAuthorization()
    {
        Scope(Constants.Scopes.Authorizations.All);
        RuleFor(x => x.Resource.Subject)
            .NotNull()
            .DependentRules(() =>
            {
                Subject(x => x.Subject!.Value);
            });
    }
}
