using WeatherAggregatorDemo.Models;
using System.Text.Json;

namespace WeatherAggregatorDemo.Services
{
    public class MetNoWeatherService : IWeatherProvider
    {
        private readonly HttpClient _httpClient;
        private readonly LocationService _locationService;
        private readonly ILogger<MetNoWeatherService> _logger;

        public string ProviderName => "YR.no";

        public MetNoWeatherService(HttpClient httpClient, LocationService locationService, ILogger<MetNoWeatherService> logger)
        {
            _httpClient = httpClient;
            _locationService = locationService;
            _logger = logger;
            
            // Met.no kräver User-Agent
            _httpClient.DefaultRequestHeaders.Add("User-Agent", AppConstants.UserAgent);
        }

        public async Task<List<WeatherData>> GetWeatherDataAsync(string location, DateTime date)
        {
            try
            {
                var (lat, lon) = await _locationService.GetCoordinatesAsync(location);
                
                var url = $"{AppConstants.MetNoApiUrl}?lat={lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lon={lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

                _logger.LogInformation($"Anropar Met.no API för {location} ({lat}, {lon})");
                
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Met.no API fel: {response.StatusCode}");
                    return new List<WeatherData>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonSerializer.Deserialize<MetNoWeatherResponse>(json);

                if (weatherResponse?.Properties?.TimeSeries == null)
                {
                    _logger.LogWarning("Ingen väderdata från Met.no");
                    return new List<WeatherData>();
                }

                var weatherData = new List<WeatherData>();
                
                // bara data för rätt dag
                var targetDate = date.Date;
                var relevantData = weatherResponse.Properties.TimeSeries
                    .Where(ts => ts.Time.Date == targetDate)
                    .ToList();

                foreach (var timeSeries in relevantData)
                {
                    var weatherEntry = new WeatherData
                    {
                        Provider = "metno",
                        Temperature = timeSeries.Data.Instant.Details.AirTemperature,
                        WindSpeed = timeSeries.Data.Instant.Details.WindSpeed,
                        WindDirection = timeSeries.Data.Instant.Details.WindFromDirection,
                        Humidity = timeSeries.Data.Instant.Details.RelativeHumidity,
                        AirPressure = timeSeries.Data.Instant.Details.AirPressureAtSeaLevel,
                        Precipitation = timeSeries.Data.Next1Hours?.Details.PrecipitationAmount ?? 0,
                        Description = TranslateSymbolCode(timeSeries.Data.Next1Hours?.Summary.SymbolCode ?? ""),
                        Hour = timeSeries.Time.Hour,
                        Timestamp = timeSeries.Time
                    };

                    weatherData.Add(weatherEntry);
                }

                _logger.LogInformation($"Hämtade {weatherData.Count} väderposer från Met.no för {location}");
                return weatherData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fel vid anrop till Met.no API: {ex.Message}");
                return new List<WeatherData>();
            }
        }

        private string TranslateSymbolCode(string symbolCode)
        {
            var translations = new Dictionary<string, string>
            {
                { "clearsky", "Klart" },
                { "fair", "Lätt molnigt" },
                { "partlycloudy", "Delvis molnigt" },
                { "cloudy", "Molnigt" },
                { "lightrain", "Lätt regn" },
                { "rain", "Regn" },
                { "heavyrain", "Kraftigt regn" },
                { "lightsnow", "Lätt snöfall" },
                { "snow", "Snö" },
                { "fog", "Dimma" }
            };

            var baseCode = symbolCode.Split('_')[0]; // Ta bort _day/_night suffix
            return translations.TryGetValue(baseCode, out var translation) ? translation : "Okänt väder";
        }
    }
}