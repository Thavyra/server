namespace Thavyra.Rest.Security.Scopes;

public class ScopeNames
{
    public static class Account
    {
        public const string Group = "account";
        public const string Profile = $"{Group}.profile";
        public const string ReadProfile = $"{Group}.profile.read";
        public const string EditProfile = $"{Group}.profile.edit";
        public const string Logins = $"{Group}.logins";
        public const string Delete = $"{Group}.delete";
    }
    
    public static class Applications
    {
        public const string Group = "applications";
        public const string Create = $"{Group}.create";
        public const string Read = $"{Group}.read";
        public const string Edit = $"{Group}.edit";
        public const string Delete = $"{Group}.delete";
    }
}