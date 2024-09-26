using OpenIddict.Abstractions;
using Thavyra.Data.Contexts;
using Thavyra.Data.Models;

namespace Thavyra.Data.Seeds;

public static class DbInitializer
{
    public static void Initialize(ThavyraDbContext context)
    {
        if (context.Applications.Any())
        {
            return;
        }

        var scopes = new[]
        {
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "account",
                DisplayName = "Full Account Access",
                Description = "View, manage and delete your profile, logins and account."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "account.profile",
                DisplayName = "Profile",
                Description = "View and modify your profile and avatar."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "account.profile.read",
                DisplayName = "View Profile",
                Description = "View your profile and avatar."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "account.transactions",
                DisplayName = "View Transactions",
                Description = "View transactions sent to your virtual balance."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "account.logins",
                DisplayName = "Logins",
                Description = "Manage your logins and change your password."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "account.delete",
                DisplayName = "Delete Account",
                Description = "Delete your account."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "applications",
                DisplayName = "Applications",
                Description = "Manage your OAuth applications."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "applications.read",
                DisplayName = "View Applications",
                Description = "View your OAuth applications."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "authorizations",
                DisplayName = "Authorizations",
                Description = "Manage your authorized third party apps."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "transactions",
                DisplayName = "Transactions",
                Description = "Send transactions and view your virtual balance."
            }
        };
        
        context.Scopes.AddRange(scopes);
        context.SaveChanges();
        
        var users = new[]
        {
            new UserDto
            {
                Id = Guid.Parse("d6c0de21-8579-44e5-8e08-c45bc03a4227"),
                Username = "Thavyra",
                Description = null,
                CreatedAt = DateTime.UtcNow
            }
        };
        
        context.Users.AddRange(users);
        context.SaveChanges();

        var applications = new[]
        {
            new ApplicationDto
            {
                Id = Guid.Parse("96ab99e3-6b3e-4265-a36a-8524e9a74f60"),
                OwnerId = Guid.Parse("d6c0de21-8579-44e5-8e08-c45bc03a4227"),

                ClientId = "j17FHt92IVSvzBufa9c1q0QDaKTd6aFC",
                ClientSecretHash = "$2a$11$tRuJytuNCD6RujInkHzoJOGuYsRrxliJXB2Q9iImeT3e9jsCUl2fS",
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,

                Type = Constants.ApplicationTypes.Service,
                Name = "Thavyra",
                Description = null,

                CreatedAt = DateTime.UtcNow
            },
            
            new ApplicationDto
            {
                Id = Guid.Parse("309ebb5f-6c89-4fe1-9ff5-b8a9ae6ae44a"),
                OwnerId = Guid.Parse("d6c0de21-8579-44e5-8e08-c45bc03a4227"),
                
                ClientId = "tLa9WBudEm1IcVjfbszcsjOAEn1tE8cC",
                ClientSecretHash = "$2a$11$JY5RAuErd67fIlxXeW5BeOm0DoQwFRFa5e2Zgu4lDEx13DiUNffMW",
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                
                Type = OpenIddictConstants.ApplicationTypes.Native,
                Name = "Account Portal",
                Description = null,
                
                CreatedAt = DateTime.UtcNow
            },
            
            
            new ApplicationDto
            {
                Id = Guid.Parse("5276f543-60f5-40ff-9bef-631f4649714a"),
                OwnerId = Guid.Parse("d6c0de21-8579-44e5-8e08-c45bc03a4227"),

                ClientId = "KD1l6XfcD5rL1b7des0IUzJ69xZ4JTYF",
                ClientSecretHash = "$2a$11$3Gy/VUv1024vxAYeqj4V2u0pIkzZDj8w0M6jNJpbyB5MQ9akxVfwm",
                ClientType = OpenIddictConstants.ClientTypes.Confidential,
                ConsentType = OpenIddictConstants.ConsentTypes.Explicit,

                Type = OpenIddictConstants.ApplicationTypes.Web,
                Name = "Confidential Client",
                Description = null,

                CreatedAt = DateTime.UtcNow
            },
            new ApplicationDto
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
            }
        };
        
        context.Applications.AddRange(applications);
        context.SaveChanges();

        var redirects = new[]
        {
            new RedirectDto
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.Parse("5276f543-60f5-40ff-9bef-631f4649714a"),
                Uri = "http://localhost:5700",
                CreatedAt = DateTime.UtcNow
            },
            new RedirectDto
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.Parse("5276f543-60f5-40ff-9bef-631f4649714a"),
                Uri = "https://oauth.pstmn.io/v1/callback",
                CreatedAt = DateTime.UtcNow
            },
            new RedirectDto
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.Parse("309ebb5f-6c89-4fe1-9ff5-b8a9ae6ae44a"),
                Uri = "http://localhost:3000/auth/callback/thavyra",
                CreatedAt = DateTime.UtcNow
            },
            new RedirectDto
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.Parse("dab3902c-499a-4e56-afd0-6c44736d1593"),
                Uri = "http://localhost:5700",
                CreatedAt = DateTime.UtcNow
            },
            new RedirectDto
            {
                Id = Guid.NewGuid(),
                ApplicationId = Guid.Parse("dab3902c-499a-4e56-afd0-6c44736d1593"),
                Uri = "https://oauth.pstmn.io/v1/callback",
                CreatedAt = DateTime.UtcNow
            }
        };
        
        context.Redirects.AddRange(redirects);
        context.SaveChanges();
    }
}