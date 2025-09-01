using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;
using System.Linq;
using System.Collections.Generic;

namespace VONetData.Pages.Network;

[Authorize(Roles="Admin,Installer")]
public class SiteModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public Site? Site { get; set; }
    public List<NetworkLink> Links { get; set; } = new();
    public SiteModel(ApplicationDbContext db) => _db = db;

    public IActionResult OnGet(int id)
    {
        Load(id);
        if (Site == null) return NotFound();
        return Page();
    }

    public IActionResult OnPostUpdate(int id)
    {
        var s = _db.Sites.FirstOrDefault(x => x.Id == id);
        if (s == null) return NotFound();
        s.Name = Request.Form["Name"];
        s.Type = Request.Form["Type"];
        s.Description = Request.Form["Description"];
        if (double.TryParse(Request.Form["Latitude"], out var la)) s.Latitude = la; else s.Latitude = null;
        if (double.TryParse(Request.Form["Longitude"], out var lo)) s.Longitude = lo; else s.Longitude = null;
        s.Address = Request.Form["Address"];
        _db.SaveChanges();
        return RedirectToPage("/Network/Site", new { id });
    }

    public IActionResult OnPostAddLink(int id)
    {
        var s = _db.Sites.FirstOrDefault(x => x.Id == id);
        if (s == null) return NotFound();
        if (int.TryParse(Request.Form["ToSiteId"], out var toId))
        {
            var link = new NetworkLink
            {
                FromSiteId = id,
                ToSiteId = toId,
                Medium = Request.Form["Medium"],
                DistanceKm = double.TryParse(Request.Form["DistanceKm"], out var dk) ? dk : null
            };
            _db.NetworkLinks.Add(link);
            _db.SaveChanges();
        }
        return RedirectToPage("/Network/Site", new { id });
    }

    public IActionResult OnPostDelete(int id)
    {
        var s = _db.Sites.FirstOrDefault(x => x.Id == id);
        if (s != null)
        {
            var links = _db.NetworkLinks.Where(l => l.FromSiteId == id || l.ToSiteId == id).ToList();
            _db.NetworkLinks.RemoveRange(links);
            _db.Sites.Remove(s);
            _db.SaveChanges();
        }
        return RedirectToPage("/Network/Index");
    }

    private void Load(int id)
    {
        Site = _db.Sites.FirstOrDefault(x => x.Id == id);
        if (Site != null)
        {
            Links = _db.NetworkLinks.Where(l => l.FromSiteId == id).ToList();
        }
    }
}