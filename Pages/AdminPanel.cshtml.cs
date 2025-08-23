using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using VONetData.Models;

namespace VONetData.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminPanelModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public List<IdentityUser> IdentityUsers { get; set; } = new();
        public List<FeatureRequest> RecentFeatureRequests { get; set; } = new();
        public HashSet<string> AdminEmails { get; set; } = new();
        public string? Message { get; set; }

        public AdminPanelModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            IdentityUsers = _userManager.Users.OrderBy(u => u.Email).ToList();
            RecentFeatureRequests = _db.FeatureRequests.OrderByDescending(f => f.Id).Take(10).ToList();
            AdminEmails = new HashSet<string>(
                (await _userManager.GetUsersInRoleAsync("Admin")).Select(u => u.Email ?? "")
            );
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var email = Request.Form["Email"].ToString();
            var action = Request.Form["action"].ToString();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                Message = "User not found.";
            }
            else if (action == "add")
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                Message = $"{email} is now an admin.";
            }
            else if (action == "remove")
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                Message = $"{email} is no longer an admin.";
            }
            // Refresh lists
            IdentityUsers = _userManager.Users.OrderBy(u => u.Email).ToList();
            RecentFeatureRequests = _db.FeatureRequests.OrderByDescending(f => f.Id).Take(10).ToList();
            AdminEmails = new HashSet<string>(
                (await _userManager.GetUsersInRoleAsync("Admin")).Select(u => u.Email ?? "")
            );
            return Page();
        }
    }
}
