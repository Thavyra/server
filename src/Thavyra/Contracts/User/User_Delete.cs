namespace Thavyra.Contracts.User;

public record User_Delete
{
    public required Guid Id { get; init; }
}