using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Transactions.Get;

public class Request : RequestWithAuthentication
{
    public Guid Id { get; set; }
}