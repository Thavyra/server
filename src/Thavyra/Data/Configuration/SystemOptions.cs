using Thavyra.Data.Models;

namespace Thavyra.Data.Configuration;

public class SystemOptions
{
    public Guid UserId { get; set; }
    public Guid ApplicationId { get; set; }

    public WelcomeTransactionOptions? WelcomeTransaction { get; set; }

    public List<Guid> DefaultPermissions { get; set; } = [];
}

public class WelcomeTransactionOptions
{
    public string? Message { get; set; }
    public double Amount { get; set; }
}