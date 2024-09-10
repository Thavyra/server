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
                Description = "View and modify your profile."
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
                Name = "account.profile.edit",
                DisplayName = "Edit Profile",
                Description = "Modify your profile and avatar."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "account.username",
                DisplayName = "Username",
                Description = "Change your username."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "account.logins",
                DisplayName = "Logins",
                Description = "Manage your logins, and change your password."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "applications",
                DisplayName = "Applications",
                Description = "View, manage and delete your OAuth applications."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "applications.create",
                DisplayName = "Create Applications",
                Description = "Create new OAuth applications."
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
                Name = "applications.edit",
                DisplayName = "Modify Applications",
                Description = "Modify your OAuth applications."
            },
            
            new ScopeDto
            {
                Id = Guid.NewGuid(),
                Name = "applications.delete",
                DisplayName = "Delete Applications",
                Description = "Delete your OAuth applications."
            }
        };
        
        context.Scopes.AddRange(scopes);
        context.SaveChanges();
        
        var users = new[]
        {
            new UserDto
            {
                Id = Guid.Parse("d6c0de21-8579-44e5-8e08-c45bc03a4227"),
                Username = "System",
                Description = "The system.",
                CreatedAt = DateTime.UtcNow
            }
        };
        
        context.Users.AddRange(users);
        context.SaveChanges();

        var applications = new[]
        {
            new ApplicationDto
            {
                Id = Guid.Parse("5276f543-60f5-40ff-9bef-631f4649714a"),
                OwnerId = Guid.Parse("d6c0de21-8579-44e5-8e08-c45bc03a4227"),

                ClientId = "KD1l6XfcD5rL1b7des0IUzJ69xZ4JTYF",
                ClientSecret = "KD1l6XfcD5rL1b7des0IUzJ69xZ4JTYF",
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