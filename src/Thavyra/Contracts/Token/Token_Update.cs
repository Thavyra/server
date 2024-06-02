namespace Thavyra.Contracts.Token;

public record Token_Update
{
    public Change<string?> ReferenceId { get; init; }
    public Change<string?> Type { get; init; }
    public Change<string?> Status { get; init; }
    public Change<string?> Payload { get; init; }

    public Change<DateTime?> RedeemedAt { get; init; }
    public Change<DateTime?> ExpiresAt { get; init; }
}
