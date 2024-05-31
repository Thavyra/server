namespace Thavyra.Contracts.User;

public record User_Create
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}