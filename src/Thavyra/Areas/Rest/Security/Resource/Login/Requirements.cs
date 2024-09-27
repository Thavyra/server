using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security.Resource.Login;

public class ReadLoginRequirement : IOperationAuthorizationRequirement;
public class SetPasswordRequirement : IOperationAuthorizationRequirement;