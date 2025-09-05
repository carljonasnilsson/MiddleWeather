using WeatherAggregatorDemo.Models;

namespace WeatherAggregatorDemo.Services
{
    public class WeatherDataAggregator
    {
        private readonly IEnumerable<IWeatherProvider> _weatherProviders;
        private readonly ILogger<WeatherDataAggregator> _logger;


        public WeatherDataAggregator(
            IEnumerable<IWeatherProvider> weatherProviders,
            ILogger<WeatherDataAggregator> logger)
        {
            _weatherProviders = weatherProviders;
            _logger = logger;
        }

        public async Task<List<AggregatedWeatherData>> GetAggregatedWeatherDataAsync(string location, DateTime date)
        {
            var tasks = new List<Task<List<WeatherData>>>();
            foreach(var provider in _weatherProviders)
            {
                tasks.Add(FetchProviderDataAsync(provider, location, date));
            }
            
            var allProviderResults = await Task.WhenAll(tasks);
            
            var allWeatherData = new List<WeatherData>();
            foreach(var result in allProviderResults)
            {
                allWeatherData.AddRange(result);
            }

            var hourlyAggregates = new List<AggregatedWeatherData>();
            var groupedByHour = allWeatherData.GroupBy(data => data.Hour);
            foreach(var hourGroup in groupedByHour)
            {
                hourlyAggregates.Add(CreateHourlyAggregate(hourGroup.ToList()));
            }
            hourlyAggregates = hourlyAggregates.OrderBy(agg => agg.Hour).ToList();

            return hourlyAggregates;
        }

        private async Task<List<WeatherData>> FetchProviderDataAsync(IWeatherProvider provider, string location, DateTime date)
        {
            try
            {
                _logger.LogInformation($"Hämtar data från {provider.ProviderName} för {location}");
                return await provider.GetWeatherDataAsync(location, date);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fel vid hämtning från {provider.ProviderName}: {ex.Message}");
                return new List<WeatherData>();
            }
        }

        private AggregatedWeatherData CreateHourlyAggregate(List<WeatherData> hourlyData)
        {
            return Medelvader.ProcessWeatherData(hourlyData);
        }
    }
}