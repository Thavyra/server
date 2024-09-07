using OpenIddict.Abstractions;
using Thavyra.Contracts.Application;
using Thavyra.Contracts.Login;
using Thavyra.Contracts.User;

namespace Thavyra.Mocks;

public static class Repository
{
    public static class Applications
    {
        public static Application ConfidentialClient { get; } = new Application
        {
            Id = Guid.Parse("5276f543-60f5-40ff-9bef-631f4649714a"),
            OwnerId = Guid.Parse("d6c0de21-8579-44e5-8e08-c45bc03a4227"),

            ClientId = "KD1l6XfcD5rL1b7des0IUzJ69xZ4JTYF",
            ClientType = OpenIddictConstants.ClientTypes.Confidential,
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,

            Type = OpenIddictConstants.ApplicationTypes.Web,
            Name = "Confidential Client",
            Description = null,

            CreatedAt = DateTime.UtcNow
        };

        public static Application PublicClient { get; } = new Application
        {
            Id = Guid.Parse("dab3902c-499a-4e56-afd0-6c44736d1593"),
            OwnerId = Guid.Parse("d6c0de21-8579-44e5-8e08-c45bc03a4227"),

            ClientId = "Sr7WsvmprCK5ZpPUbNtZvnrcetJyay90",
            ClientType = OpenIddictConstants.ClientTypes.Public,
            ConsentType = OpenIddictConstants.ConsentTypes.Explicit,

            Type = OpenIddictConstants.ApplicationTypes.Native,
            Name = "Public Client",
            Description = null,

            CreatedAt = DateTime.UtcNow
        };
        
        public static readonly IReadOnlyDictionary<string, Application> List = new Dictionary<string, Application>
        {
            { ConfidentialClient.Id.ToString(), ConfidentialClient },
            { PublicClient.Id.ToString(), PublicClient }
        };
    }
    
    public static class Users
    {
        public static IDictionary<string, User> List = new Dictionary<string, User>
        {
            
        };

        public static IDictionary<Guid, string> Passwords = new Dictionary<Guid, string>();
    }
}