namespace Thavyra.Contracts.Transaction;

/// <summary>
/// 
/// </summary>
/// <returns><see cref="Transaction"/>, <see cref="InsufficientBalance"/></returns>
public record Transaction_Create
{
    public Guid ApplicationId { get; init; }
    public Guid SubjectId { get; init; }
    public string? Description { get; init; }
    public double Amount { get; init; }
}