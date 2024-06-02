namespace Thavyra.Contracts.User;

public record User_Update
{
    public required Guid Id { get; init; }
    
    public Change<string> Username { get; init; }
    public Change<string> Password { get; init; }
    public Change<string?> Description { get; init; }
}