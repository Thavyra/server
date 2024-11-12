using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Transactions.PostTransfer;

public class Request : UserRequest
{
    public Guid RecipientId { get; set; }
    public JsonOptional<string?> Description { get; set; }
    public double Amount { get; set; }
}