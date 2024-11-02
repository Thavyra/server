using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Users;

public class UserResponse
{
    public required string Id { get; set; }
    public required string? Username { get; set; }
    
    /// <summary>
    /// Requires <c>account.profile.read</c>
    /// </summary>
    public JsonOptional<string?> Description { get; set; }
    
    public JsonOptional<double> Balance { get; set; }
}