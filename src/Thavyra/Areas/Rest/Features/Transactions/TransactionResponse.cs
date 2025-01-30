using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Transactions;

public class TransactionResponse
{
    /// <summary>
    /// Whether the transaction represents a transfer to another user.
    /// </summary>
    public required bool IsTransfer { get; set; }
    
    /// <summary>
    /// id of the transaction.
    /// </summary>
    public required Guid Id { get; set; }
    /// <summary>
    /// id of the client that requested the transaction.
    /// </summary>
    public required Guid ApplicationId { get; set; }
    /// <summary>
    /// id of the subject of the transaction.
    /// </summary>
    public required Guid SubjectId { get; set; }
    /// <summary>
    /// If the transaction is a transfer, the user to whom the amount was transferred.
    /// </summary>
    public JsonOptional<Guid> RecipientId { get; set; }
    /// <summary>
    /// Description of the transaction, can be null.
    /// </summary>
    public required JsonNullable<string> Description { get; set; }
    /// <summary>
    /// The difference made to the user's balance, unless the transaction is a transfer in which case the value will represent the change in the recipient's balance.
    /// </summary>
    public required double Amount { get; set; }
    /// <summary>
    /// When the transaction was created.
    /// </summary>
    public required DateTime CreatedAt { get; set; }
}