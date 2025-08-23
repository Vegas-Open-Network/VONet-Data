using System.ComponentModel.DataAnnotations;

namespace VONetData.Models
{
    public class FeatureRequest
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string? Email { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
