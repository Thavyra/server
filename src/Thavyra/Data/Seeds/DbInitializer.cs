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
                ApplicationId = Guid.Parse("dab3902c-499a-4e56-afd0-6c44736d1593"),
                Uri = "http://localhost:5700",
                CreatedAt = DateTime.UtcNow
            }
        };
        
        context.Redirects.AddRange(redirects);
        context.SaveChanges();
    }
}