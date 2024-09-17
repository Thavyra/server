namespace Thavyra.Contracts.User.Register;

public record User_Register
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}