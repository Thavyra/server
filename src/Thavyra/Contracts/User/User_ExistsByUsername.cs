namespace Thavyra.Contracts.User;

public record User_ExistsByUsername
{
    public required string Username { get; init; }
}