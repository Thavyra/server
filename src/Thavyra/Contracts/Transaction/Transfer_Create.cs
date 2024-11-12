namespace Thavyra.Contracts.Transaction;

/// <summary>
/// 
/// </summary>
/// <returns><see cref="Transaction"/>, <see cref="InsufficientBalance"/></returns>
public record Transfer_Create
{
    public Guid ApplicationId { get; init; }
    public Guid SubjectId { get; init; }
    public Guid RecipientId { get; init; }

    public string? Description { get; init; }
    public double Amount { get; init; }
}