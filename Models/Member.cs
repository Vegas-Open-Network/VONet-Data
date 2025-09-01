using System.ComponentModel.DataAnnotations;

namespace VONetData.Models
{
    public class Member
    {
        public int Id { get; set; }

        // Personal Info
        [Required]
        public string FirstName { get; set; } = string.Empty;
        [Required]
        public string LastName { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;

        // Address Info
        [Required]
        public string StreetAddress { get; set; } = string.Empty;
        public string? Unit { get; set; }
        [Required]
        public string City { get; set; } = string.Empty;
        [Required]
        public string State { get; set; } = string.Empty;
        [Required]
        public string ZipCode { get; set; } = string.Empty;
        public bool RoofAccess { get; set; }
        public string? Referral { get; set; }
        public bool AcceptedLicense { get; set; }

        // Optional: Associated Identity User
        public string? UserId { get; set; } // Foreign key to AspNetUsers.Id

        // Mapping (optional manual entry for now)
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        // CRM workflow
        [MaxLength(64)]
        public string? Status { get; set; }
        [MaxLength(256)]
        public string? AssignedToEmail { get; set; }
    }
}
