using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security.Resource.Objective;

public class CreateObjectiveRequirement : IOperationAuthorizationRequirement;
public class ReadObjectiveRequirement : IOperationAuthorizationRequirement;
public class UpdateObjectiveRequirement : IOperationAuthorizationRequirement;
public class DeleteObjectiveRequirement : IOperationAuthorizationRequirement;