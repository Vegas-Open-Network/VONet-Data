using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;
using System.Linq;
using System.Text.Json;

namespace VONetData.Pages
{
    public class MapModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        public string MemberGeoJson { get; set; } = "{}";

        public MapModel(ApplicationDbContext db)
        {
            _db = db;
        }

        public void OnGet()
        {
            var points = _db.Members
                .Where(m => m.Latitude.HasValue && m.Longitude.HasValue)
                .Select(m => new {
                    type = "Feature",
                    geometry = new { type = "Point", coordinates = new [] { m.Longitude!.Value, m.Latitude!.Value } },
                    properties = new { address = m.StreetAddress + ", " + m.City, roof = m.RoofAccess }
                }).ToList();
            var fc = new { type = "FeatureCollection", features = points };
            MemberGeoJson = JsonSerializer.Serialize(fc);
        }
    }
}
