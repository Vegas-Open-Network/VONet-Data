using System;
using System.ComponentModel.DataAnnotations;

namespace VONetData.Models;

public class InteractionLog
{
    public int Id { get; set; }
    [Required]
    public int MemberId { get; set; }
    [Required]
    [MaxLength(64)]
    public string Type { get; set; } = string.Empty; // email, call, site_visit, install
    [MaxLength(256)]
    public string? PerformedByEmail { get; set; }
    [MaxLength(2000)]
    public string? Summary { get; set; }
    public DateTime OccurredUtc { get; set; } = DateTime.UtcNow;
}
