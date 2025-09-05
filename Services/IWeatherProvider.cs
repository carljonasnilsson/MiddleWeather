using WeatherAggregatorDemo.Models;

namespace WeatherAggregatorDemo.Services
{
    public interface IWeatherProvider
    {
        string ProviderName { get; }
        Task<List<WeatherData>> GetWeatherDataAsync(string location, DateTime date);
    }
}