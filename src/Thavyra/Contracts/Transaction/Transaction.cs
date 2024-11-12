namespace Thavyra.Contracts.Transaction;

public record Transaction
{
    public required Guid Id { get; init; }
    public required Guid ApplicationId { get; init; }
    public required Guid SubjectId { get; init; }
    public Guid? RecipientId { get; init; }

    public required bool IsTransfer { get; init; }
    public required string? Description { get; init; }
    public required double Amount { get; init; }

    public required DateTime CreatedAt { get; init; }
}