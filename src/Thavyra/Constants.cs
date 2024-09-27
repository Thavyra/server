using OpenIddict.Abstractions;

namespace Thavyra;

public static class Constants
{
    public static class Scopes
    {
        public const string Sudo = "sudo";
        
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
        public static class ConsentTypes
        {
            public const string Implicit = $"{Prefixes.ConsentType}{OpenIddictConstants.ConsentTypes.Implicit}";
        }
        
        public static class Prefixes
        {
            public const string ConsentType = "cst:";
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
}