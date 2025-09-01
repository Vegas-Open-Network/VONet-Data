using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;
using VONetData.Services;

namespace VONetData.Pages.Network;

[Authorize(Roles="Admin,Installer")]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IGeocodingService _geocoder;

    [BindProperty]
    public Site Site { get; set; } = new();

    public CreateModel(ApplicationDbContext db, IGeocodingService geocoder)
    {
        _db = db;
        _geocoder = geocoder;
    }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        bool hasCoords = Site.Latitude.HasValue && Site.Longitude.HasValue;
        if (!hasCoords && !string.IsNullOrWhiteSpace(Site.Address))
        {
            var (lat, lon) = await _geocoder.GeocodeFreeformAsync(Site.Address!, ct);
            if (lat.HasValue && lon.HasValue)
            {
                Site.Latitude = lat;
                Site.Longitude = lon;
            }
        }

        // After geocode attempt, ensure we have either coordinates or address (model validation guarantees address or coords provided)
        if (!(Site.Latitude.HasValue && Site.Longitude.HasValue) && string.IsNullOrWhiteSpace(Site.Address))
        {
            ModelState.AddModelError(string.Empty, "Provide either coordinates or an address.");
            return Page();
        }

        _db.Sites.Add(Site);
        await _db.SaveChangesAsync(ct);
        return RedirectToPage("Index");
    }
}
