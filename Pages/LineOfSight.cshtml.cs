using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VONetData.Pages
{
    public class LineOfSightModel : PageModel
    {
        [BindProperty]
        public string Address { get; set; } = string.Empty;
        public string? ResultMessage { get; set; }

        public void OnGet() { }

        public void OnPost()
        {
            if (string.IsNullOrWhiteSpace(Address))
            {
                ResultMessage = "Please enter a valid address.";
                return;
            }
            // For demo: just echo the address
            ResultMessage = $"Checking line of sight for: {Address}";
        }
    }
}
