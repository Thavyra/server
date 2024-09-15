using Bogus;
using OpenIddict.Abstractions;
using Thavyra.Data.Models;

namespace Thavyra.Tests.Data;

public static class Bogus
{
    public static List<UserDto> User(int count = 1, Guid? id = null)
    {
        return new Faker<UserDto>()
            .RuleFor(u => u.Id, f => id ?? f.Random.Guid())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Description, f => f.Lorem.Paragraph())
            .RuleFor(u => u.CreatedAt, _ => DateTime.UtcNow)
            .Generate(count);
    }

    public static List<PasswordLoginDto> Password(int count = 1, Guid? id = null, Guid? userId = null)
    {
        return new Faker<PasswordLoginDto>()
            .RuleFor(p => p.Id, f => id ?? f.Random.Guid())
            .RuleFor(p => p.UserId, f => userId ?? f.Random.Guid())
            .RuleFor(p => p.Password, f => f.Internet.Password())
            .RuleFor(p => p.CreatedAt, _ => DateTime.UtcNow)
            .Generate(count);
    }

    public static List<ApplicationDto> Application(
        int count = 1, 
        UserDto? owner = null,
        Guid? id = null,
        string? name = null,
        string? type = null,
        string? clientId = null,
        string? clientType = null,
        string? clientSecret = null,
        string? consentType = null)
    {
        string[] applicationTypes =
            [OpenIddictConstants.ApplicationTypes.Web, OpenIddictConstants.ApplicationTypes.Native];

        string[] consentTypes = [OpenIddictConstants.ConsentTypes.Explicit, OpenIddictConstants.ConsentTypes.Implicit];
        
        return new Faker<ApplicationDto>()
            .RuleFor(a => a.Owner, _ => owner ?? User()[0])
            
            .RuleFor(a => a.Id, f => id ?? f.Random.Guid())
            .RuleFor(a => a.OwnerId, f => owner?.Id ?? f.Random.Guid())
            .RuleFor(a => a.Type, f => type ?? f.PickRandom(applicationTypes))
            .RuleFor(a => a.Name, f => name ?? f.Company.CompanyName())
            .RuleFor(a => a.ClientId, f => clientId ?? f.Random.AlphaNumeric(32))
            .RuleFor(a => a.ClientType, (_, a) => clientType ?? a.Type switch
            {
                OpenIddictConstants.ApplicationTypes.Web => OpenIddictConstants.ClientTypes.Confidential,
                _ => OpenIddictConstants.ClientTypes.Public
            })
            .RuleFor(a => a.ClientSecret, (f, a) => a.ClientType switch
            {
                OpenIddictConstants.ClientTypes.Confidential => clientSecret ?? f.Random.Utf16String(),
                _ => null
            })
            .RuleFor(a => a.ConsentType, f => consentType ?? f.PickRandom(consentTypes))
            .RuleFor(a => a.Description, f => f.Lorem.Paragraph())
            .RuleFor(a => a.CreatedAt, _ => DateTime.UtcNow)
            .Generate(count);
    }
}