using System.Text.Json.Serialization;

namespace WeatherAggregatorDemo.Models
{
    // Met.no API models
    public class MetNoWeatherResponse
    {
        [JsonPropertyName("properties")]
        public MetNoProperties Properties { get; set; } = new();
    }

    public class MetNoProperties
    {
        [JsonPropertyName("timeseries")]
        public List<MetNoTimeSeries> TimeSeries { get; set; } = new();
    }

    public class MetNoTimeSeries
    {
        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("data")]
        public MetNoData Data { get; set; } = new();
    }

    public class MetNoData
    {
        [JsonPropertyName("instant")]
        public MetNoInstant Instant { get; set; } = new();

        [JsonPropertyName("next_1_hours")]
        public MetNoNext1Hours? Next1Hours { get; set; }
    }

    public class MetNoInstant
    {
        [JsonPropertyName("details")]
        public MetNoDetails Details { get; set; } = new();
    }

    public class MetNoDetails
    {
        [JsonPropertyName("air_temperature")]
        public double AirTemperature { get; set; }

        [JsonPropertyName("relative_humidity")]
        public double RelativeHumidity { get; set; }

        [JsonPropertyName("wind_speed")]
        public double WindSpeed { get; set; }

        [JsonPropertyName("wind_from_direction")]
        public double WindFromDirection { get; set; }

        [JsonPropertyName("air_pressure_at_sea_level")]
        public double AirPressureAtSeaLevel { get; set; }
    }

    public class MetNoNext1Hours
    {
        [JsonPropertyName("summary")]
        public MetNoSummary Summary { get; set; } = new();

        [JsonPropertyName("details")]
        public MetNoNextDetails Details { get; set; } = new();
    }

    public class MetNoSummary
    {
        [JsonPropertyName("symbol_code")]
        public string SymbolCode { get; set; } = "";
    }

    public class MetNoNextDetails
    {
        [JsonPropertyName("precipitation_amount")]
        public double PrecipitationAmount { get; set; }
    }

    // SMHI API models
    public class SmhiWeatherResponse
    {
        [JsonPropertyName("timeSeries")]
        public List<SmhiTimeSeries> TimeSeries { get; set; } = new();
    }

    public class SmhiTimeSeries
    {
        [JsonPropertyName("validTime")]
        public DateTime ValidTime { get; set; }

        [JsonPropertyName("parameters")]
        public List<SmhiParameter> Parameters { get; set; } = new();
    }

    public class SmhiParameter
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("values")]
        public List<double> Values { get; set; } = new();
    }

    // OpenWeatherMap API models
    public class OpenWeatherMapResponse
    {
        [JsonPropertyName("list")]
        public List<OpenWeatherMapForecast> List { get; set; } = new();
    }

    public class OpenWeatherMapForecast
    {
        [JsonPropertyName("dt")]
        public long Dt { get; set; }

        [JsonPropertyName("main")]
        public OpenWeatherMapMain Main { get; set; } = new();

        [JsonPropertyName("weather")]
        public List<OpenWeatherMapWeather> Weather { get; set; } = new();

        [JsonPropertyName("wind")]
        public OpenWeatherMapWind Wind { get; set; } = new();

        [JsonPropertyName("rain")]
        public OpenWeatherMapRain? Rain { get; set; }
    }

    public class OpenWeatherMapMain
    {
        [JsonPropertyName("temp")]
        public double Temp { get; set; }

        [JsonPropertyName("humidity")]
        public double Humidity { get; set; }

        [JsonPropertyName("pressure")]
        public double Pressure { get; set; }
    }

    public class OpenWeatherMapWeather
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
    }

    public class OpenWeatherMapWind
    {
        [JsonPropertyName("speed")]
        public double Speed { get; set; }

        [JsonPropertyName("deg")]
        public double Deg { get; set; }
    }

    public class OpenWeatherMapRain
    {
        [JsonPropertyName("3h")]
        public double ThreeHour { get; set; }
    }
}