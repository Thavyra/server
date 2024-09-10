using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security.Scopes;

namespace Thavyra.Rest.Security;

public static class Policies
{
    public static class Operation
    {
        public class Application
        {
            public const string Create = "operation.application.create";
            public const string Read = "operation.application.read";
            public const string Update = "operation.application.update";
            public const string Delete = "operation.application.delete";
        }
    
        public static class User
        {
            public const string Read = "operation.user.read";
            public const string Update = "operation.user.update";
            public const string Username = "operation.user.username";
            public const string Password = "operation.user.login.password";
            public const string Delete = "operation.user.delete";
        }
    }
}
