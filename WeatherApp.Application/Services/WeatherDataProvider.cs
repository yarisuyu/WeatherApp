using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Services
{
    public class WeatherDataProvider : IWeatherDataProvider
    {
        private readonly IWeatherApiClient _apiClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WeatherDataProvider> _logger;

        public WeatherDataProvider(IWeatherApiClient apiClient, IMemoryCache cache, ILogger<WeatherDataProvider> logger)
        {
            _apiClient = apiClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<WeatherData> GetCurrentWeatherAsync(double lat, double lon, CancellationToken ct = default)
        {
            string cacheKey = $"weather_{lat}_{lon}";

            if (_cache.TryGetValue(cacheKey, out WeatherData? cachedData) && cachedData is not null)
            {
                _logger.LogInformation("Возвращаем данные из кэша");
                return cachedData;
            }

            _logger.LogInformation("Кэш пуст, запрашиваем API");
            var data = await _apiClient.GetCurrentWeatherAsync(lat, lon, ct);

            // Кэшируем на 5 минут (можно вынести в настройки)
            _cache.Set(cacheKey, data, TimeSpan.FromMinutes(5));

            return data;
        }

        public async Task<ForecastData> GetForecastAsync(double lat, double lon, int dayCount, CancellationToken ct = default)
        {
            string cacheKey = $"forecast_{lat}_{lon}";

            if (_cache.TryGetValue(cacheKey, out ForecastData? cachedData) && cachedData is not null)
            {
                _logger.LogInformation("Возвращаем данные из кэша");
                return cachedData;
            }

            _logger.LogInformation("Кэш пуст, запрашиваем API");
            var data = await _apiClient.GetForecastAsync(lat, lon, dayCount, ct);

            // Кэшируем на 60 минут (можно вынести в настройки)
            _cache.Set(cacheKey, data, TimeSpan.FromMinutes(60));

            return data;
        }
    }
}
