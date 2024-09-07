namespace Thavyra.Contracts.Token;

/// <summary>
/// Updates the specified token.
/// </summary>
/// <returns><see cref="Token"/></returns>
public record Token_Update
{
    public required Guid Id { get; init; }

    public Change<string?> ReferenceId { get; init; }
    public Change<string?> Type { get; init; }
    public Change<string?> Status { get; init; }
    public Change<string?> Payload { get; init; }

    public Change<DateTime?> RedeemedAt { get; init; }
    public Change<DateTime?> ExpiresAt { get; init; }
}
