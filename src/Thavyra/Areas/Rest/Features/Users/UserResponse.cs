using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Users;

public class UserResponse
{
    /// <summary>
    /// id of the user.
    /// </summary>
    public required Guid Id { get; set; }
    
    /// <summary>
    /// Username of the user.
    /// </summary>
    public required string Username { get; set; }
    
    /// <summary>
    /// User's profile description.
    /// </summary>
    public JsonOptional<string?> Description { get; set; }
    
    /// <summary>
    /// Virtual account balance of the user.
    /// </summary>
    public JsonOptional<double> Balance { get; set; }

    /// <summary>
    /// When the user created their account.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}