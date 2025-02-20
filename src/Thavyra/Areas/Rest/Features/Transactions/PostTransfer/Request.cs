using Thavyra.Rest.Documentation;
using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Transactions.PostTransfer;

[SchemaName("CreateTransferRequest")]
public class Request : UserRequest
{
    public Guid RecipientId { get; set; }
    public JsonOptional<string?> Description { get; set; }
    public double Amount { get; set; }
}