using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;
using BC = BCrypt.Net.BCrypt;

namespace BCrypt;

[Command(Description = "Computes BCrypt hash of a string.")]
public class BCryptCommand : ICommand
{
    [CommandParameter(0, Description = "String to hash.")]
    public required string Value { get; set; }

    [CommandOption("rounds", 'r')] 
    public int Rounds { get; set; } = 11;
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        string hash = BC.HashPassword(Value, BC.GenerateSalt(Rounds));
        
        console.Output.WriteLine(hash);

        return default;
    }
}