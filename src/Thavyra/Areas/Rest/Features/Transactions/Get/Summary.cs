using FastEndpoints;
using Thavyra.Rest.Documentation;

namespace Thavyra.Rest.Features.Transactions.Get;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Get Transaction";
        Description = "Returns a transaction object for the given id.";
        
        Response(example: Example.Transaction("Transaction"));
    }
}