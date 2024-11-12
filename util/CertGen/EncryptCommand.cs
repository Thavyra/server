using System.Security.Cryptography.X509Certificates;
using CliFx;
using CliFx.Attributes;
using CliFx.Infrastructure;

namespace CertGen;

[Command("encrypt")]
public class EncryptCommand : ICommand
{
    [CommandParameter(order: 0, IsRequired = false)]
    public string? Name { get; set; }
    
    public ValueTask ExecuteAsync(IConsole console)
    {
        var certificate = CertGen.Generate(Name ?? "Thavyra",
            notBefore: DateTimeOffset.UtcNow,
            notAfter: DateTimeOffset.UtcNow.AddYears(2),
            X509KeyUsageFlags.KeyEncipherment);
        
        var bytes = certificate.Export(X509ContentType.Pfx);
        
        var base64 = Convert.ToBase64String(bytes);
        
        console.Output.WriteLine();
        
        console.Output.WriteLine($"Encryption Certificate '{certificate.SubjectName.Name}'");
        
        console.Output.WriteLine();
        console.Output.WriteLine(base64);
        console.Output.WriteLine();

        return default;
    }
}