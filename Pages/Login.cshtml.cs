using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace VONetData.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        public LoginModel(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();
        public string? ErrorMessage { get; set; }

        public class InputModel
        {
            public string Email { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Input.Email) || string.IsNullOrWhiteSpace(Input.Password))
            {
                ErrorMessage = "Email and password are required.";
                return Page();
            }
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ErrorMessage = "Invalid login attempt.";
                return Page();
            }
            var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                return RedirectToPage("/Index");
            }
            ErrorMessage = "Invalid login attempt.";
            return Page();
        }
    }
}
