namespace WeatherAggregatorDemo.Models
{
    public class WeatherViewModel
    {
        public string Location { get; set; } = string.Empty;
        public DateTime CurrentTime { get; set; }
        public int NextHour { get; set; }
        public string ShowingHour { get; set; } = string.Empty;
        public AggregatedWeatherData? WeatherData { get; set; }
    }
}