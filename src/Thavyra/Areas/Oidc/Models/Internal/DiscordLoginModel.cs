using System.Text.Json.Serialization;

namespace Thavyra.Oidc.Models.Internal;

public class DiscordLoginModel
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("username")]
    public required string Username { get; set; }
}