using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;

namespace VONetData.Pages
{
    public class FeatureRequestModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public FeatureRequest Request { get; set; } = new FeatureRequest();
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }

        public FeatureRequestModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            Success = false;
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill out all required fields.";
                return Page();
            }
            try
            {
                Request.SubmittedAt = DateTime.UtcNow;
                _db.FeatureRequests.Add(Request);
                _db.SaveChanges();
                Success = true;
                ModelState.Clear();
                Request = new FeatureRequest();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error saving your request: " + ex.Message;
            }
            return Page();
        }
    }
}
