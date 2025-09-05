using WeatherAggregatorDemo.Models;
using System.Text.Json;

namespace WeatherAggregatorDemo.Services
{
    public class SmhiWeatherService : IWeatherProvider
    {
        private readonly HttpClient _httpClient;
        private readonly LocationService _locationService;
        private readonly ILogger<SmhiWeatherService> _logger;

        public string ProviderName => "SMHI";

        public SmhiWeatherService(HttpClient httpClient, LocationService locationService, ILogger<SmhiWeatherService> logger)
        {
            _httpClient = httpClient;
            _locationService = locationService;
            _logger = logger;
        }

        public async Task<List<WeatherData>> GetWeatherDataAsync(string location, DateTime date)
        {
            try
            {
                var (lat, lon) = await _locationService.GetCoordinatesAsync(location);
                var url = $"{AppConstants.SmhiApiUrl}/lon/{lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}/lat/{lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}/data.json";

                _logger.LogInformation($"Anropar SMHI API för {location} ({lat}, {lon})");
                
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"SMHI API fel: {response.StatusCode}");
                    return new List<WeatherData>();
                }

                var json = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonSerializer.Deserialize<SmhiWeatherResponse>(json);

                if (weatherResponse?.TimeSeries == null)
                {
                    _logger.LogWarning("Ingen väderdata från SMHI");
                    return new List<WeatherData>();
                }

                var weatherData = new List<WeatherData>();
                
                // bara data för den dagen vi vill ha
                var targetDate = date.Date;
                var relevantData = weatherResponse.TimeSeries
                    .Where(ts => ts.ValidTime.Date == targetDate)
                    .ToList();

                foreach (var timeSeries in relevantData)
                {
                    var weatherEntry = new WeatherData
                    {
                        Provider = ProviderName.ToLower(),
                        Temperature = GetParameterValue(timeSeries.Parameters, "t"),
                        WindSpeed = GetParameterValue(timeSeries.Parameters, "ws"),
                        WindDirection = GetParameterValue(timeSeries.Parameters, "wd"),
                        Humidity = GetParameterValue(timeSeries.Parameters, "r"),
                        AirPressure = GetParameterValue(timeSeries.Parameters, "msl"),
                        Precipitation = GetParameterValue(timeSeries.Parameters, "pmean"),
                        WindGust = GetParameterValue(timeSeries.Parameters, "gust"),
                        TotalCloudCover = (int)GetParameterValue(timeSeries.Parameters, "tcc_mean"),
                        Description = TranslateWeatherSymbol((int)GetParameterValue(timeSeries.Parameters, "Wsymb2")),
                        Hour = timeSeries.ValidTime.Hour,
                        Timestamp = timeSeries.ValidTime
                    };

                    weatherData.Add(weatherEntry);
                }

                _logger.LogInformation($"Hämtade {weatherData.Count} väderposer från SMHI för {location}");
                return weatherData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fel vid anrop till SMHI API: {ex.Message}");
                return new List<WeatherData>();
            }
        }

        private double GetParameterValue(List<SmhiParameter> parameters, string name)
        {
            var param = parameters.FirstOrDefault(p => p.Name == name);
            return param?.Values?.FirstOrDefault() ?? 0.0;
        }

        private string TranslateWeatherSymbol(int symbol)
        {
            return symbol switch
            {
                1 => "Klart",
                2 => "Lätt molnighet",
                3 => "Halvklart",
                4 => "Molnigt",
                5 => "Mycket molnigt",
                6 => "Mulet",
                7 => "Dimma",
                8 => "Lätt regnskurar",
                9 => "Måttliga regnskurar",
                10 => "Kraftiga regnskurar",
                11 => "Åska",
                12 => "Lätt snöblandad regn",
                13 => "Måttlig snöblandad regn",
                14 => "Kraftig snöblandad regn",
                15 => "Lätt snöfall",
                16 => "Måttligt snöfall",
                17 => "Kraftigt snöfall",
                18 => "Lätt regn",
                19 => "Måttligt regn",
                20 => "Kraftigt regn",
                21 => "Åska",
                _ => "Okänt väder"
            };
        }
    }
}