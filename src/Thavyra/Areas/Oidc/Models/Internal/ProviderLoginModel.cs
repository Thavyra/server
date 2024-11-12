using System.Text.Json.Serialization;

namespace Thavyra.Oidc.Models.Internal;

public interface IProviderLoginModel
{
    string AccountId { get; }
    string Username { get; }
    string AvatarUrl { get; }
}

public record DefaultProviderLoginModel(string AccountId, string Username, string AvatarUrl) : IProviderLoginModel;

public readonly struct InvalidProviderLoginModel : IProviderLoginModel
{
    public string AccountId => throw new NotSupportedException();
    public string Username => throw new NotSupportedException();
    public string AvatarUrl => throw new NotSupportedException();
}

public class DiscordLoginModel : IProviderLoginModel
{
    [JsonPropertyName("id")]
    public required string AccountId { get; set; }
    
    [JsonPropertyName("username")]
    public required string Username { get; set; }

    [JsonPropertyName("avatar")]
    public required string AvatarHash { get; set; }

    public string AvatarUrl => $"https://cdn.discordapp.com/avatars/{AccountId}/{AvatarHash}.png";
}
