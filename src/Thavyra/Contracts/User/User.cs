namespace Thavyra.Contracts.User;

public record User
{
    public required string Id { get; init; }
    public required string Username { get; init; }
    public required string? Description { get; init; }
}