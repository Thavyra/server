using Thavyra.Contracts.Application;
using Thavyra.Contracts.User;

namespace Thavyra.Rest.Features;

public class AuthenticationState
{
    public User? User { get; set; }
    public Application? Application { get; set; }
}