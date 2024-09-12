using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Transactions;

public class TransactionResponse
{
    public required bool IsTransfer { get; set; }
    
    public required Guid Id { get; set; }
    public required Guid ApplicationId { get; set; }
    public required Guid SubjectId { get; set; }
    public JsonOptional<Guid> RecipientId { get; set; }
    
    public required JsonNullable<string> Description { get; set; }
    public required double Amount { get; set; }

    public required DateTime CreatedAt { get; set; }
}