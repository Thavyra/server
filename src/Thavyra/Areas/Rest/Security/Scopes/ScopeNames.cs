namespace Thavyra.Rest.Security.Scopes;

public class ScopeNames
{
    public static class Account
    {
        public const string Group = "account";
        public const string Profile = "account.profile";
        public const string ReadProfile = "account.profile.read";
        public const string EditProfile = "account.profile.edit";
        public const string Username = "account.username";
        public const string Logins = "account.logins";
        public const string Delete = "account.delete";
    }
    
    public static class Applications
    {
        public const string Group = "applications";
        public const string Create = "applications.create";
        public const string Read = "applications.read";
        public const string Edit = "applications.edit";
        public const string Delete = "applications.delete";
    }
}