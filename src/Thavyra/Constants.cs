using OpenIddict.Abstractions;

namespace Thavyra;

public static class Constants
{
    public static class Scopes
    {
        public const string Sudo = "sudo";
        public const string Admin = "admin";
        
        public static class Account
        {
            public const string All = "account";
            public const string Profile = "account.profile";
            public const string ReadProfile = "account.profile.read";
            public const string ReadTransactions = "account.transactions";
            public const string Logins = "account.logins";
        }
    
        public static class Applications
        {
            public const string All = "applications";
            public const string Read = "applications.read";
        }
    
        public static class Authorizations
        {
            public const string All = "authorizations";
            public const string Read = "authorizations.read";
        }
    
        public static class Transactions
        {
            public const string All = "transactions";
        }
    }
    
    public static class Permissions
    {
        public const string Setup = "setup";
        
        public static class Scopes
        {
            public const string Sudo = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Sudo;
            public const string Admin = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Admin;
            
            public static class Account
            {
                public const string All = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Account.All;
                public const string Profile = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Account.Profile;
                public const string ReadProfile = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Account.ReadProfile;
                public const string ReadTransactions = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Account.ReadTransactions;
                public const string Logins = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Account.Logins;
            }
            
            public static class Applications
            {
                public const string All = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Applications.All;
                public const string Read = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Applications.Read;
            }
            
            public static class Authorizations
            {
                public const string All = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Authorizations.All;
                public const string Read = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Authorizations.Read;
            }
            
            public static class Transactions
            {
                public const string All = OpenIddictConstants.Permissions.Prefixes.Scope + Constants.Scopes.Transactions.All;
            }
        }
        
        public static class ConsentTypes
        {
            public const string Implicit = Prefixes.ConsentType + OpenIddictConstants.ConsentTypes.Implicit;
        }
        
        public static class Prefixes
        {
            public const string ConsentType = "cst:";
            public const string Operation = "op:";
        }
    }

    public static class ApplicationTypes
    {
        public const string Service = "service";
    }
    
    public static class Claims
    {
        public const string ApplicationId = "application_id";
    }
    
    public static class Roles
    {
        public const string Admin = "admin";
    }
}