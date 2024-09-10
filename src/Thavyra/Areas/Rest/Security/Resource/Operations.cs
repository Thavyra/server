using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace Thavyra.Rest.Security.Resource;

public static class Operations
{
    public static readonly OperationAuthorizationRequirement Create = new() { Name = nameof(Create) };
    public static readonly OperationAuthorizationRequirement Read = new() { Name = nameof(Read) };
    public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
    public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
    
    public static class User
    {
        /// <summary>
        /// Change user's username.
        /// </summary>
        public static readonly OperationAuthorizationRequirement Username = new() {Name = "Username"};
    
        /// <summary>
        /// Change user's password.
        /// </summary>
        public static readonly OperationAuthorizationRequirement Password = new () {Name = "Password"};
    }
}