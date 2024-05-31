namespace Thavyra.Contracts.User;

public record User_GetById
{
    public required string Id { get; init; }
}