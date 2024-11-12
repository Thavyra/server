using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MassTransit;

namespace Thavyra.Data.Models;

[Table("transactions")]
public class TransactionDto
{
    [Column("id")]
    public Guid Id { get; set; } = NewId.NextGuid();
    
    [Column("application_id")]
    public Guid ApplicationId { get; set; }
    
    [Column("subject_id")]
    public Guid SubjectId { get; set; }
    
    [Column("recipient_id")]
    public Guid? RecipientId { get; set; }

    [Column("description"), MaxLength(400)]
    public string? Description { get; set; }
    
    [Column("amount")]
    public double Amount { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationDto? Application { get; set; }
    public UserDto? Subject { get; set; }
    public UserDto? Recipient { get; set; }
}