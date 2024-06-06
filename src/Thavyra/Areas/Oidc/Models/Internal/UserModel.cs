namespace Thavyra.Oidc.Models.Internal;

public class UserModel
{
    public required Guid Id { get; set; }
    public required string Username { get; set; }
    public required DateTime CreatedAt { get; set; }
}