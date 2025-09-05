using Microsoft.AspNetCore.Mvc;
using WeatherAggregatorDemo.Models;
using WeatherAggregatorDemo.Services;

namespace WeatherAggregatorDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherDataAggregator _aggregator;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(WeatherDataAggregator aggregator, ILogger<WeatherController> logger)
        {
            _aggregator = aggregator;
            _logger = logger;
        }

        [HttpGet("aggregated")]
        public async Task<ActionResult<List<AggregatedWeatherData>>> GetAggregatedWeather(
            string location = "Stockholm", 
            string? date = null)
        {
            try
            {
                var targetDate = string.IsNullOrEmpty(date) 
                    ? DateTime.Today 
                    : DateTime.Parse(date);

                _logger.LogInformation($"Hämtar aggregerad väderdata för {location} på {targetDate:yyyy-MM-dd}");

                var result = await _aggregator.GetAggregatedWeatherDataAsync(location, targetDate);
                
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fel vid hämtning av väderdata: {ex.Message}");
                return StatusCode(500, "Ett fel uppstod vid hämtning av väderdata");
            }
        }

        [HttpGet("demo/{hour}")]
        public async Task<ActionResult<object>> GetWeatherDemo(int hour, string location = "Stockholm")
        {
            try
            {
                var aggregatedData = await _aggregator.GetAggregatedWeatherDataAsync(location, DateTime.Today);
                var hourData = aggregatedData.FirstOrDefault(d => d.Hour == hour);

                if (hourData == null)
                {
                    return NotFound($"Ingen data för timme {hour}");
                }

                return Ok(new
                {
                    Concept = "Weather Data Aggregation Demo",
                    Description = "Hämtar data från flera väder-providers och skapar medelvärden",
                    Location = location,
                    Hour = hour,
                    AggregatedData = new
                    {
                        hourData.AverageTemperature,
                        hourData.AverageWindSpeed,
                        hourData.AverageHumidity,
                        hourData.TotalPrecipitation,
                        hourData.MostCommonDescription
                    },
                    IndividualProviders = hourData.ProviderData.Select(p => new
                    {
                        p.Provider,
                        p.Temperature,
                        p.WindSpeed,
                        p.Humidity,
                        p.Precipitation,
                        p.Description
                    }).ToList(),
                    Explanation = "Medelvärdet skapas från alla providers. Klicka på 'Individual Providers' för att se varje källa."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fel vid demo: {ex.Message}");
                return StatusCode(500, "Ett fel uppstod");
            }
        }
    }
}