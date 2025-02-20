using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Transactions.GetByUser;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get User Transactions";
        Description = "Returns the transactions of which the user is the subject or recipient.";

        Response(example: new[] { Example.Transaction("Transaction"), Example.Transaction("Transfer") });
        Response(404, "User Not Found");
    }
}