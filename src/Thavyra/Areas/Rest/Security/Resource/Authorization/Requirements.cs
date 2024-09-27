using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security.Resource.Authorization;

public class ReadAuthorizationRequirement : IOperationAuthorizationRequirement;
public class DeleteAuthorizationRequirement : IOperationAuthorizationRequirement;