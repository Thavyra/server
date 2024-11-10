namespace Thavyra.Data.Configuration;

public class UserOptions
{
    public WelcomeTransaction WelcomeTransaction { get; set; } = new();
    public FallbackUsernames FallbackUsernames { get; set; } = new();
}

public class WelcomeTransaction
{
    public string? Message { get; set; }
    public double? Amount { get; set; }
}

public class FallbackUsernames
{
    public IReadOnlyList<string> Adjectives { get; set; } = [];
    public IReadOnlyList<string> Nouns { get; set; } = [];
}