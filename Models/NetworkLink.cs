using System.ComponentModel.DataAnnotations;

namespace VONetData.Models;

public class NetworkLink
{
    public int Id { get; set; }
    public int FromSiteId { get; set; }
    public int ToSiteId { get; set; }
    [MaxLength(32)] public string? Medium { get; set; } // wireless, fiber, etc.
    public double? DistanceKm { get; set; }
}
