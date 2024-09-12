using System.ComponentModel.DataAnnotations.Schema;

namespace Thavyra.Data.Models;

[Table("transactions")]
public class TransactionDto
{
    [Column("id")]
    public Guid Id { get; set; }
    [Column("application_id")]
    public Guid ApplicationId { get; set; }
    [Column("subject_id")]
    public Guid SubjectId { get; set; }
    [Column("recipient_id")]
    public Guid? RecipientId { get; set; }

    [Column("description")]
    public string? Description { get; set; }
    [Column("amount")]
    public double Amount { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    public ApplicationDto Application { get; set; } = default!;
    public UserDto Subject { get; set; } = default!;
    public UserDto? Recipient { get; set; } = default!;
}