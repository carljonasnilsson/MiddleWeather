using System.Text.Json;

namespace WeatherAggregatorDemo.Services
{
    public class LocationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LocationService> _logger;

        public LocationService(HttpClient httpClient, ILogger<LocationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<(double lat, double lon)> GetCoordinatesAsync(string cityName)
        {
            // Hårdkodade koordinater för vanliga nordiska städer för demo
            var cities = new Dictionary<string, (double lat, double lon)>(StringComparer.OrdinalIgnoreCase)
            {
                { "Stockholm", (59.3293, 18.0686) },
                { "Göteborg", (57.7089, 11.9746) },
                { "Malmö", (55.6059, 13.0007) },
                { "Oslo", (59.9139, 10.7522) },
                { "Bergen", (60.3913, 5.3221) },
                { "Trondheim", (63.4305, 10.3951) },
                { "Helsinki", (60.1699, 24.9384) },
                { "Tampere", (61.4991, 23.7871) },
                { "Turku", (60.4518, 22.2666) },
                { "Köpenhamn", (55.6761, 12.5683) },
                { "Copenhagen", (55.6761, 12.5683) },
                { "Aarhus", (56.1629, 10.2039) },
                { "Reykjavik", (64.1466, -21.9426) }
            };

            if (cities.TryGetValue(cityName.Trim(), out var coordinates))
            {
                _logger.LogInformation($"Hittade koordinater för {cityName}: {coordinates.lat}, {coordinates.lon}");
                return coordinates;
            }

            // Fallback till Stockholm om staden inte hittas
            _logger.LogWarning($"Stad '{cityName}' hittades inte, använder Stockholm som fallback");
            return cities["Stockholm"];
        }
    }
}