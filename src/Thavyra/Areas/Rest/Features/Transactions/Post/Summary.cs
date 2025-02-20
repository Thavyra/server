using FastEndpoints;

namespace Thavyra.Rest.Features.Transactions.Post;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Summary = "Create Transaction";
        Description = "This endpoint is likely to be merged with the Send Transfer endpoint. Documentation will be provided once this endpoint has been finalised.";
    }
}