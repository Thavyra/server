namespace Thavyra.Contracts.Transaction;

/// <summary>
/// 
/// </summary>
/// <returns><see cref="Transaction"/>, <see cref="NotFound"/></returns>
public record Transaction_GetById
{
    public Guid Id { get; init; }
}