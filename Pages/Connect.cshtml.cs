using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;
using Microsoft.AspNetCore.Identity;

namespace VONetData.Pages
{
    public class ConnectModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        [BindProperty]
        public Member Member { get; set; } = new Member();
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public ConnectModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public void OnGet()
        {
            Success = false;
        }

        public IActionResult OnPost()
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
                // If user is logged in, associate their UserId
                var userId = _userManager.GetUserId(User);
                if (!string.IsNullOrEmpty(userId))
                {
                    Member.UserId = userId;
                }
                _db.Members.Add(Member);
                _db.SaveChanges();
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
