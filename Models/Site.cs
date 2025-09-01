using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace VONetData.Models;

public class Site : IValidatableObject
{
    public int Id { get; set; }
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    [Required, MaxLength(32)] public string Type { get; set; } = "node"; // supernode, node, candidate
    [MaxLength(200)] public string? Description { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    [MaxLength(256)] public string? Address { get; set; }

    // Validation: must supply either (Latitude & Longitude) or Address
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        bool hasCoords = Latitude.HasValue && Longitude.HasValue;
        bool hasAddress = !string.IsNullOrWhiteSpace(Address);
        if (!hasCoords && !hasAddress)
        {
            yield return new ValidationResult(
                "Provide either both Latitude & Longitude or an Address.",
                new[] { nameof(Latitude), nameof(Longitude), nameof(Address) });
        }
        if ((Latitude.HasValue ^ Longitude.HasValue))
        {
            yield return new ValidationResult(
                "Latitude and Longitude must both be supplied if one is supplied.",
                new[] { nameof(Latitude), nameof(Longitude) });
        }
    }
}
