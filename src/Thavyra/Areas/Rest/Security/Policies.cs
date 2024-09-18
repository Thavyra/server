using Microsoft.AspNetCore.Authorization;
using Thavyra.Rest.Security.Scopes;

namespace Thavyra.Rest.Security;

public static class Policies
{
    public static class Operation
    {
        public static class Application
        {
            public const string Create = "operation.application.create";
            public const string Read = "operation.application.read";
            public const string Update = "operation.application.update";
            public const string Delete = "operation.application.delete";
        }
        
        public static class Authorization
        {
            public const string Read = "operation.authorization.read";
            public const string Delete = "operation.authorization.delete";
        }
    
        public static class User
        {
            public const string Read = "operation.user.read";
            public const string Update = "operation.user.update";
            public const string Username = "operation.user.username";
            public const string Delete = "operation.user.delete";
        }
        
        public static class Login
        {
            public const string Read = "operation.user.login.read";
            public const string Password = "operation.user.login.password";
        }
        
        public static class Transaction
        {
            public const string Read = "operation.transaction.read";
            public const string Send = "operation.transaction.send";
            public const string Transfer = "operation.transaction.transfer";
        }
        
        public static class Objective
        {
            public const string Create = "operation.objective.create";
            public const string Read = "operation.objective.read";
            public const string Update = "operation.objective.update";
            public const string Delete = "operation.objective.delete";
        }
        
        public static class Score
        {
            public const string Create = "operation.score.create";
            public const string Read = "operation.score.read";
            public const string Delete = "operation.score.delete";
        }
    }
}
