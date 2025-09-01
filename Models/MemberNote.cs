using System;
using System.ComponentModel.DataAnnotations;

namespace VONetData.Models;

public class MemberNote
{
    public int Id { get; set; }
    [Required]
    public int MemberId { get; set; }
    [Required]
    [MaxLength(4000)]
    public string Text { get; set; } = string.Empty;
    [MaxLength(256)]
    public string? AuthorEmail { get; set; }
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
