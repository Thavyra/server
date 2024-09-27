using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security.Resource.Score;

public class CreateScoreRequirement : IOperationAuthorizationRequirement;
public class ReadScoreRequirement : IOperationAuthorizationRequirement;
public class DeleteScoreRequirement : IOperationAuthorizationRequirement;