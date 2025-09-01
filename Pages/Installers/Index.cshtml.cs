using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace VONetData.Pages.Installers;

[Authorize(Roles="Admin,Installer")]
public class IndexModel : PageModel
{
    public void OnGet() { }
}