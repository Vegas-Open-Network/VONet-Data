namespace VONetData.Services;

public interface IGeocodingService
{
    Task<(double? lat, double? lon)> GeocodeAsync(string street, string? unit, string city, string state, string zip, CancellationToken ct = default);
    Task<(double? lat, double? lon)> GeocodeFreeformAsync(string address, CancellationToken ct = default);
}