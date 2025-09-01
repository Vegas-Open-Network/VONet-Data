using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;

namespace VONetData.Pages.Network;

[Authorize(Roles="Admin,Installer")]
public class SiteCreateModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public SiteCreateModel(ApplicationDbContext db) => _db = db;

    [BindProperty] public Site Input { get; set; } = new();
    public string? Error { get; set; }

    public void OnGet() { }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            Error = "Validation failed.";
            return Page();
        }
        _db.Sites.Add(Input);
        _db.SaveChanges();
        return RedirectToPage("/Network/Index");
    }
}
