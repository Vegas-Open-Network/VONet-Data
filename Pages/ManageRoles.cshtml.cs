using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace VONetData.Pages
{
    public class ManageRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        public ManageRolesModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;
        public IdentityUser? FoundUser { get; set; }
        public bool IsAdmin { get; set; }
        public string? Message { get; set; }

        public async Task OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Message = "Please enter an email.";
                return;
            }
            FoundUser = await _userManager.FindByEmailAsync(Email);
            if (FoundUser == null)
            {
                Message = "User not found.";
                return;
            }
            IsAdmin = await _userManager.IsInRoleAsync(FoundUser, "Admin");
            var action = Request.Form["action"];
            if (action == "add" && !IsAdmin)
            {
                await _userManager.AddToRoleAsync(FoundUser, "Admin");
                Message = "Admin role added.";
                IsAdmin = true;
            }
            else if (action == "remove" && IsAdmin)
            {
                await _userManager.RemoveFromRoleAsync(FoundUser, "Admin");
                Message = "Admin role removed.";
                IsAdmin = false;
            }
        }
    }
}
