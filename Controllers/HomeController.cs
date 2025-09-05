using Microsoft.AspNetCore.Mvc;
using WeatherAggregatorDemo.Models;
using WeatherAggregatorDemo.Services;

namespace WeatherAggregatorDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly WeatherDataAggregator _aggregator;
        private readonly ILogger<HomeController> _logger;

        public HomeController(WeatherDataAggregator aggregator, ILogger<HomeController> logger)
        {
            _aggregator = aggregator;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string location = "Stockholm")
        {
            try
            {
                var now = DateTime.Now;
                var nextHour = now.Hour + 1;
                if (nextHour >= 24) nextHour = 23;

                var today = DateTime.Today;
                var aggregatedData = await _aggregator.GetAggregatedWeatherDataAsync(location, today);
                
                var nextHourData = aggregatedData.FirstOrDefault(d => d.Hour == nextHour);
                
                var model = new WeatherViewModel
                {
                    Location = location,
                    CurrentTime = now,
                    NextHour = nextHour,
                    WeatherData = nextHourData,
                    ShowingHour = $"{nextHour:00}:00"
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fel vid hämtning av väderdata: {ex.Message}");
                return View("Error");
            }
        }
    }
}