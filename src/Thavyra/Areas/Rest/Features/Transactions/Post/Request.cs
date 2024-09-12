using Thavyra.Rest.Features.Users;
using Thavyra.Rest.Json;

namespace Thavyra.Rest.Features.Transactions.Post;

public class Request : UserRequest
{
    public JsonOptional<string?> Description { get; set; }
    public double Amount { get; set; }
}