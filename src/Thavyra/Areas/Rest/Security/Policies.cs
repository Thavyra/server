using Microsoft.AspNetCore.Authorization;

namespace Thavyra.Rest.Security;

public static class Policies
{
    public static class Operation
    {
        public static class Application
        {
            public const string Create = "operation.application.create";
            public const string Read = "operation.application.read";
            public const string ReadObjectives = "operation.application.objectives.read";
            public const string ReadTransactions = "operation.application.transactions.read";
            public const string Update = "operation.application.update";
            public const string ResetClientSecret = "operation.application.client_secret";
            public const string Delete = "operation.application.delete";
        }
        
        public static class Authorization
        {
            public const string Read = "operation.authorization.read";
            public const string Delete = "operation.authorization.delete";
        }
    
        public static class User
        {
            public const string ReadProfile = "operation.user.read";
            public const string ReadBalance = "operation.user.balance.read";
            public const string ReadApplications = "operation.user.applications.read";
            public const string ReadAuthorizations = "operation.user.authorizations.read";
            public const string ReadLogins = "operation.user.logins.read";
            public const string ReadTransactions = "operation.user.transactions.read";
            public const string UpdateProfile = "operation.user.update";
            public const string ChangeUsername = "operation.user.username";
            public const string Delete = "operation.user.delete";
        }
        
        public static class Login
        {
            public const string Read = "operation.login.read";
            public const string SetPassword = "operation.login.password";
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
