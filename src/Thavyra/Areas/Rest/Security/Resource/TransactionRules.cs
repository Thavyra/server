using Microsoft.AspNetCore.Authorization;
using Thavyra.Contracts.Transaction;

namespace Thavyra.Rest.Security.Resource;

public class ReadTransactionRequirement : IOperationAuthorizationRequirement;
public class SendTransactionRequirement : IOperationAuthorizationRequirement;
public class SendTransferRequirement : IOperationAuthorizationRequirement;

public class SubjectCanSendTransaction : FluentAuthorizationHandler<SendTransactionRequirement, Transaction_Create>
{
    public SubjectCanSendTransaction()
    {
        Scope(Constants.Scopes.Transactions.All);
        Subject(x => x.SubjectId);
    }
}

public class SubjectCanSendTransfer : FluentAuthorizationHandler<SendTransferRequirement, Transfer_Create>
{
    public SubjectCanSendTransfer()
    {
        Scope(Constants.Scopes.Transactions.All);
        Subject(x => x.SubjectId);
    }
}

public class
    SubjectOrRecipientCanReadTransaction : FluentAuthorizationHandler<ReadTransactionRequirement, Transaction>
{
    public SubjectOrRecipientCanReadTransaction()
    {
        Scope(Constants.Scopes.Account.ReadTransactions);
    }
    
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ReadTransactionRequirement requirement,
        Transaction resource)
    {
        if (context.User.HasSubject(resource.SubjectId))
        {
            context.Succeed(requirement);
        }

        if (resource.RecipientId.HasValue && context.User.HasSubject(resource.RecipientId.Value))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
