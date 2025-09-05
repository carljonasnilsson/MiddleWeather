using WeatherAggregatorDemo.Models;
using System.Text.Json;

namespace WeatherAggregatorDemo.Services
{
    public class OpenWeatherMapService : IWeatherProvider
    {
        private readonly HttpClient _httpClient;
        private readonly LocationService _locationService;
        private readonly ILogger<OpenWeatherMapService> _logger;
        private readonly IConfiguration _configuration;

        public string ProviderName => "OpenWeatherMap";

        public OpenWeatherMapService(HttpClient httpClient, LocationService locationService, ILogger<OpenWeatherMapService> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _locationService = locationService;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<WeatherData>> GetWeatherDataAsync(string location, DateTime date)
        {
            var apiKey = _configuration["WeatherSettings:OpenWeatherMapApiKey"];
            
            if (string.IsNullOrEmpty(apiKey) || apiKey == "DIN_API_NYCKEL_HÄR")
            {
                _logger.LogInformation($"⚠️ Simulerar OpenWeatherMap för {location} - ingen API-nyckel i appsettings.json");
                return await GetSimulatedDataAsync(location, date);
            }

            try
            {
                var (lat, lon) = await _locationService.GetCoordinatesAsync(location);
                
                var url = $"{AppConstants.OpenWeatherMapApiUrl}?lat={lat}&lon={lon}&appid={apiKey}&units=metric&lang=sv";

                _logger.LogInformation($"Anropar OpenWeatherMap API med riktig nyckel för {location} ({lat}, {lon})");
                
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"OpenWeatherMap API fel: {response.StatusCode}");
                    return new List<WeatherData>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonSerializer.Deserialize<OpenWeatherMapResponse>(json);

                if (weatherResponse?.List == null)
                {
                    _logger.LogWarning("Ingen väderdata från OpenWeatherMap");
                    return new List<WeatherData>();
                }

                var weatherData = new List<WeatherData>();
                
                // OWM bara ger data var tredje timme
                var targetDate = date.Date;
                
                var dayForecasts = weatherResponse.List
                    .Select(f => new {
                        Time = DateTimeOffset.FromUnixTimeSeconds(f.Dt).ToLocalTime().DateTime,
                        Forecast = f
                    })
                    .Where(f => f.Time.Date == targetDate)
                    .ToList();

                // fixa data för alla 24 timmar
                for (int hour = 0; hour < 24; hour++)
                {
                    var targetTime = targetDate.AddHours(hour);
                    
                    var nearestForecast = dayForecasts
                        .OrderBy(f => Math.Abs((f.Time - targetTime).TotalSeconds))
                        .FirstOrDefault();

                    if (nearestForecast != null)
                    {
                        var forecast = nearestForecast.Forecast;
                        var weatherEntry = new WeatherData
                        {
                            Provider = "openweathermap",
                            Temperature = forecast.Main.Temp,
                            WindSpeed = forecast.Wind.Speed,
                            WindDirection = forecast.Wind.Deg,
                            Humidity = forecast.Main.Humidity,
                            AirPressure = forecast.Main.Pressure,
                            Precipitation = forecast.Rain?.ThreeHour ?? 0, 
                            Description = forecast.Weather.FirstOrDefault()?.Description ?? "Okänt väder",
                            Hour = hour,
                            Timestamp = targetTime
                        };

                        weatherData.Add(weatherEntry);
                    }
                }

                _logger.LogInformation($"Hämtade {weatherData.Count} väderposer från OpenWeatherMap för {location}");
                return weatherData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fel vid anrop till OpenWeatherMap API: {ex.Message}");
                return new List<WeatherData>();
            }
        }

        // simulerad data när ingen API-nyckel finns
        private async Task<List<WeatherData>> GetSimulatedDataAsync(string location, DateTime date)
        {
            await Task.Delay(80);
            
            var random = new Random(location.GetHashCode() + date.DayOfYear);
            var weatherData = new List<WeatherData>();

            for (int hour = 0; hour < 24; hour++)
            {
                weatherData.Add(new WeatherData
                {
                    Provider = "openweathermap",
                    Temperature = 18 + random.NextDouble() * 6 - 1, // 17-23°C  
                    WindSpeed = 3 + random.NextDouble() * 6,
                    WindDirection = random.Next(0, 360),
                    Humidity = 60 + random.NextDouble() * 30,
                    Precipitation = random.NextDouble() * 1.5,
                    AirPressure = 1013 + random.NextDouble() * 15,
                    Description = GetSimulatedDescription(random, hour),
                    Hour = hour,
                    Timestamp = date.Date.AddHours(hour)
                });
            }

            return weatherData;
        }

        private string GetSimulatedDescription(Random random, int hour)
        {
            var descriptions = new[] { "Soligt", "Lätt molnigt", "Molnigt", "Regn", "Mulet" };
            return descriptions[random.Next(descriptions.Length)];
        }
    }
}