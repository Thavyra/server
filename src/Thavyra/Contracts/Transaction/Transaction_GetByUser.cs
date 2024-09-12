namespace Thavyra.Contracts.Transaction;

public record Transaction_GetByUser
{
    public Guid UserId { get; init; }
}