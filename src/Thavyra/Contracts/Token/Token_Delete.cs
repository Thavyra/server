namespace Thavyra.Contracts.Token;

/// <summary>
/// Deletes the specified token.
/// </summary>
public record Token_Delete
{
    public required Guid Id { get; init; }
}