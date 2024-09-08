namespace Thavyra.Contracts.User;

public record User_GetById
{
    public required Guid Id { get; init; }
}