using Bogus;
using OpenIddict.Abstractions;
using Thavyra.Rest.Features.Applications;
using Thavyra.Rest.Features.Authorizations;
using Thavyra.Rest.Features.Logins;
using Thavyra.Rest.Features.Redirects;
using Thavyra.Rest.Features.Transactions;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Documentation;

public static class Example
{
    public static UserResponse User() => new Faker<UserResponse>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.Username, f => f.Internet.UserName())
        .RuleFor(x => x.Description, f => f.Hacker.Phrase())
        .RuleFor(x => x.Balance, f => f.Random.Int(0, 1_000_000))
        .RuleFor(x => x.CreatedAt, f => f.Date.Past())
        .Generate();

    public static ApplicationResponse Application() => new Faker<ApplicationResponse>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.OwnerId, f => f.Random.Guid())
        .RuleFor(x => x.Name, f => f.Commerce.ProductName())
        .RuleFor(x => x.Description, f => (JsonNullable<string>) f.Hacker.Phrase())
        .RuleFor(x => x.ClientId, f => f.Random.AlphaNumeric(32))
        .RuleFor(x => x.IsConfidential, f => f.Random.Bool())
        .RuleFor(x => x.CreatedAt, f => f.Date.Past())
        .Generate();

    public static RedirectResponse Redirect() => new Faker<RedirectResponse>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.ApplicationId, f => f.Random.Guid())
        .RuleFor(x => x.Uri, _ => "https://example.com/callback")
        .RuleFor(x => x.CreatedAt, f => f.Date.Past())
        .Generate();

    public static AuthorizationResponse Authorization() => new Faker<AuthorizationResponse>()
        .RuleFor(x => x.Id, f => f.Random.Guid())
        .RuleFor(x => x.UserId, f => f.Random.Guid())
        .RuleFor(x => x.ApplicationId, f => f.Random.Guid())
        .RuleFor(x => x.Type, _ => (JsonNullable<string>) OpenIddictConstants.AuthorizationTypes.Permanent)
        .RuleFor(x => x.Status, _ => (JsonNullable<string>) OpenIddictConstants.Statuses.Valid)
        .RuleFor(x => x.CreatedAt, f => f.Date.Past())
        .Generate();

    public static IReadOnlyList<LoginResponse> Logins()
    {
        var faker = new Faker<LoginResponse>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.UsedAt, f => f.Date.Recent())
            .RuleSet(Constants.LoginTypes.Password, ruleSet => ruleSet
                .RuleFor(x => x.Type, _ => Constants.LoginTypes.Password)
                .RuleFor(x => x.ChangedAt, f => f.Date.Past()))
            .RuleSet(Constants.LoginTypes.Discord, ruleSet => ruleSet
                .RuleFor(x => x.Type, _ => Constants.LoginTypes.Discord)
                .RuleFor(x => x.ProviderUsername, f => f.Internet.UserName())
                .RuleFor(x => x.ProviderAvatarUrl, f => f.Internet.Avatar()))
            .RuleSet(Constants.LoginTypes.GitHub, ruleSet => ruleSet
                .RuleFor(x => x.Type, _ => Constants.LoginTypes.GitHub)
                .RuleFor(x => x.ProviderUsername, f => f.Internet.UserName())
                .RuleFor(x => x.ProviderAvatarUrl, f => f.Internet.Avatar()));

        return
        [
            faker.Generate($"default,{Constants.LoginTypes.Password}"), 
            faker.Generate($"default,{Constants.LoginTypes.Discord}"),
            faker.Generate($"default,{Constants.LoginTypes.GitHub}")
        ];
    }

    public static TransactionResponse Transaction(string type)
    {
        return new Faker<TransactionResponse>()
            .RuleFor(x => x.Id, f => f.Random.Guid())
            .RuleFor(x => x.ApplicationId, f => f.Random.Guid())
            .RuleFor(x => x.SubjectId, f => f.Random.Guid())
            .RuleFor(x => x.CreatedAt, f => f.Date.Past())
            .RuleSet("Transfer", ruleSet => ruleSet
                .RuleFor(x => x.IsTransfer, _ => true)
                .RuleFor(x => x.RecipientId, f => f.Random.Guid())
                .RuleFor(x => x.Amount, f => f.Random.Int(1, 10_000)))
            .RuleSet("Transaction", ruleSet => ruleSet
                .RuleFor(x => x.IsTransfer, _ => false)
                .RuleFor(x => x.RecipientId, _ => new JsonOptional<Guid>())
                .RuleFor(x => x.Amount, f => f.Random.Int(-10_000, -1)))
            .Generate($"default,{type}");
    }
}