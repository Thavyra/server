using FastEndpoints;

namespace Thavyra.Rest.Features.Transactions.PostTransfer;

public class Summary : Summary<Endpoint>
{
    public Summary()
    {
        Description =
            "This endpoint is likely to be merged with the Send Transaction endpoint. Documentation will be provided once this endpoint has been finalised.";
    }
}