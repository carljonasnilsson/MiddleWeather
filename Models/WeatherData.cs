namespace WeatherAggregatorDemo.Models
{
    public class WeatherData
    {
        public string Provider { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        public double WindDirection { get; set; }
        public double Humidity { get; set; }
        public double Precipitation { get; set; }
        public double AirPressure { get; set; }
        public double WindGust { get; set; }
        public int TotalCloudCover { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Hour { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class AggregatedWeatherData
    {
        public double AverageTemperature { get; set; }
        public double AverageWindSpeed { get; set; }
        public double AverageHumidity { get; set; }
        public double TotalPrecipitation { get; set; }
        public double AverageAirPressure { get; set; }
        public string MostCommonDescription { get; set; } = string.Empty;
        public string PrognosSakerhet { get; set; } = string.Empty; // säkerhetsgrad baserat på överensstämmelse
        public List<WeatherData> ProviderData { get; set; } = new();
        public int Hour { get; set; }
        public DateTime Timestamp { get; set; }
    }
}