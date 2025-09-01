using System.Net.Http.Json;
using System.Web;

namespace VONetData.Services;

public class NominatimGeocodingService : IGeocodingService
{
    private readonly HttpClient _http;
    public NominatimGeocodingService(HttpClient http) => _http = http;

    public async Task<(double? lat, double? lon)> GeocodeAsync(string street, string? unit, string city, string state, string zip, CancellationToken ct = default)
    {
        // Build address query
        var fullStreet = string.IsNullOrWhiteSpace(unit) ? street : $"{street} {unit}";
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["street"] = fullStreet;
        query["city"] = city;
        query["state"] = state;
        query["postalcode"] = zip;
        query["country"] = "USA";
        query["format"] = "json";
        query["limit"] = "1";
        var url = "https://nominatim.openstreetmap.org/search?" + query.ToString();
        try
        {
            var results = await _http.GetFromJsonAsync<List<NominatimResult>>(url, cancellationToken: ct);
            var first = results?.FirstOrDefault();
            if (first != null && double.TryParse(first.lat, out var la) && double.TryParse(first.lon, out var lo))
            {
                return (la, lo);
            }
        }
        catch
        {
            // swallow errors, caller will handle nulls
        }
        return (null, null);
    }

    public async Task<(double? lat, double? lon)> GeocodeFreeformAsync(string address, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(address)) return (null, null);
        var query = HttpUtility.ParseQueryString(string.Empty);
        query["q"] = address;
        query["format"] = "json";
        query["limit"] = "1";
        var url = "https://nominatim.openstreetmap.org/search?" + query.ToString();
        try
        {
            var results = await _http.GetFromJsonAsync<List<NominatimResult>>(url, cancellationToken: ct);
            var first = results?.FirstOrDefault();
            if (first != null && double.TryParse(first.lat, out var la) && double.TryParse(first.lon, out var lo))
            {
                return (la, lo);
            }
        }
        catch
        {
        }
        return (null, null);
    }

    private record NominatimResult(string lat, string lon);
}