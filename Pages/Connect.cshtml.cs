using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;
using Microsoft.AspNetCore.Identity;
using VONetData.Services;

namespace VONetData.Pages
{
    public class ConnectModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IGeocodingService _geocoder;
        [BindProperty]
        public Member Member { get; set; } = new Member();
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public ConnectModel(ApplicationDbContext db, UserManager<IdentityUser> userManager, IGeocodingService geocoder)
        {
            _db = db;
            _userManager = userManager;
            _geocoder = geocoder;
        }

        public void OnGet()
        {
            Success = false;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!Member.AcceptedLicense)
            {
                ErrorMessage = "You must agree to the Network Commons License.";
                return Page();
            }
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill out all required fields.";
                return Page();
            }
            try
            {
                var userId = _userManager.GetUserId(User);
                if (!string.IsNullOrEmpty(userId))
                {
                    Member.UserId = userId;
                }
                // Geocode
                var (lat, lon) = await _geocoder.GeocodeAsync(Member.StreetAddress, Member.Unit, Member.City, Member.State, Member.ZipCode);
                Member.Latitude = lat;
                Member.Longitude = lon;

                _db.Members.Add(Member);
                await _db.SaveChangesAsync();
                Success = true;
                ModelState.Clear();
                Member = new Member();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error saving your submission: " + ex.Message;
            }
            return Page();
        }
    }
}
