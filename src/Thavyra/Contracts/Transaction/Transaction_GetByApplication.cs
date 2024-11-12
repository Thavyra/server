namespace Thavyra.Contracts.Transaction;

public record Transaction_GetByApplication
{
    public Guid ApplicationId { get; init; }
}