namespace Thavyra.Data.Configuration;

public class UserOptions
{
    public WelcomeTransaction WelcomeTransaction { get; set; } = new();
}

public class WelcomeTransaction
{
    public string? Message { get; set; }
    public double? Amount { get; set; }
}