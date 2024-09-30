namespace Thavyra.Rest.Security.Resource.Permission;

public class ManagePermissionRequirement : IOperationAuthorizationRequirement;
public class GrantPermissionRequirement : ManagePermissionRequirement;
public class DenyPermissionRequirement : ManagePermissionRequirement;