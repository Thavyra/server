namespace Thavyra.Oidc.Models;

public class TokenModel
{
    public Guid? ApplicationId { get; set; }
    public Guid? AuthorizationId { get; set; }
    public DateTimeOffset? CreationDate { get; set; }
    public DateTimeOffset? ExpirationDate { get; set; }
    public required Guid Id { get; set; }
    public string? Payload { get; set; }
    public DateTimeOffset? RedemptionDate { get; set; }
    public string? ReferenceId { get; set; }
    public string? Status { get; set; }
    public Guid? Subject { get; set; }
    public string? Type { get; set; }
}