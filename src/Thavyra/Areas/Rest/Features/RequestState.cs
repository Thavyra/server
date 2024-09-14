using Thavyra.Contracts.Application;
using Thavyra.Contracts.User;

namespace Thavyra.Rest.Features;

public class RequestState
{
    public User? User { get; set; }
    public Application? Application { get; set; }
}