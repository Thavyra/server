using Thavyra.Rest.Security;

namespace Thavyra.Rest.Features.Transactions.Get;

public class Request : RequestWithAuthentication
{
    /// <summary>
    /// Transaction id.
    /// </summary>
    public Guid Id { get; set; }
}