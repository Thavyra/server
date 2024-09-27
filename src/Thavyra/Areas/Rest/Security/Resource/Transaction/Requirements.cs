using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security.Resource.Transaction;

public class ReadTransactionRequirement : IOperationAuthorizationRequirement;
public class SendTransactionRequirement : IOperationAuthorizationRequirement;
public class SendTransferRequirement : IOperationAuthorizationRequirement;