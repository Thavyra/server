using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CertGen;

public static class CertGen
{
    public static X509Certificate2 Generate(string serverName, DateTimeOffset notBefore, DateTimeOffset notAfter, X509KeyUsageFlags keyUsageFlags)
    {
        using var algorithm = RSA.Create();

        var purpose = keyUsageFlags switch
        {
            X509KeyUsageFlags.DigitalSignature => "Signing",
            X509KeyUsageFlags.KeyEncipherment => "Encryption",
            _ => "General"
        };
        
        var subject = new X500DistinguishedName($"CN={serverName} Server {purpose} Certificate");
        var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        
        request.CertificateExtensions.Add(new X509KeyUsageExtension(keyUsageFlags, critical: true));

        var certificate = request.CreateSelfSigned(notBefore, notAfter);

        return certificate;
    }
}