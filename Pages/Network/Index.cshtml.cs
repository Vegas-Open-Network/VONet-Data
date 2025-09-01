using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;
using System.Collections.Generic;
using System.Linq;

namespace VONetData.Pages.Network;

[Authorize(Roles="Admin,Installer")]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public List<Site> Sites { get; set; } = new();
    public List<NetworkLink> Links { get; set; } = new();
    public IndexModel(ApplicationDbContext db) => _db = db;
    public void OnGet()
    {
        Sites = _db.Sites.OrderBy(s=>s.Name).Take(200).ToList();
        Links = _db.NetworkLinks.Take(200).ToList();
    }
}