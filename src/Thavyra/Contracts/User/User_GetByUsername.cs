namespace Thavyra.Contracts.User;

public record User_GetByUsername
{
    public required string Username { get; init; }
}