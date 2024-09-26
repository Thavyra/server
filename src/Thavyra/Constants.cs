namespace Thavyra;

public static class Constants
{
    public static class Scopes
    {
        public static class Account
        {
            public const string Group = "account";
            public const string Profile = "account.profile";
            public const string ReadProfile = "account.profile.read";
            public const string ReadTransactions = "account.transactions";
            public const string Logins = "account.logins";
            public const string Delete = "account.delete";
        }
    
        public static class Applications
        {
            public const string Group = "applications";
            public const string Read = "applications.read";
        }
    
        public static class Authorizations
        {
            public const string Group = "authorizations";
        }
    
        public static class Transactions
        {
            public const string Group = "transactions";
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