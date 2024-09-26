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
        public static readonly OperationAuthorizationRequirement Username = new() { Name = nameof(Username) };
    }
    
    public static class Login
    {
        /// <summary>
        /// Change user's password.
        /// </summary>
        public static readonly OperationAuthorizationRequirement Password = new() { Name = nameof(Password) };
    }

    public static class Application
    {
        /// <summary>
        /// Set/read client secret.
        /// </summary>
        public static readonly OperationAuthorizationRequirement ClientSecret = new() { Name = nameof(ClientSecret) };
    }

    public static class Transaction
    {
        /// <summary>
        /// Send a transaction between the subject and client.
        /// </summary>
        public static readonly OperationAuthorizationRequirement Send = new() { Name = nameof(Send) };
        
        /// <summary>
        /// Create a transfer between the subject and another user.
        /// </summary>
        public static readonly OperationAuthorizationRequirement Transfer = new() { Name = nameof(Transfer) };
    }
}