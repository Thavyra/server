using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Logins;

public class LoginResponse
{
    public required string Id { get; set; }
    public required string Type { get; set; }

    public JsonOptional<string> ProviderUsername { get; set; }
    public JsonOptional<string> ProviderAvatarUrl { get; set; }
    
    public required DateTime UsedAt { get; set; }
    public JsonOptional<DateTime> ChangedAt { get; set; }
}