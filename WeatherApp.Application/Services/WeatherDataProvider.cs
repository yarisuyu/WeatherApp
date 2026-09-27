using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Services
{
    public class WeatherDataProvider : IWeatherDataProvider
    {
        private readonly IWeatherApiClient _apiClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<WeatherDataProvider> _logger;
        private readonly TimeProvider _timeProvider;
        private readonly WeatherDataProviderOptions _options;

        public WeatherDataProvider(
            IWeatherApiClient apiClient, 
            IMemoryCache cache, 
            ILogger<WeatherDataProvider> logger,
            TimeProvider timeProvider,
            IOptions<WeatherDataProviderOptions> options)
        {
            _apiClient = apiClient;
            _cache = cache;
            _logger = logger;
            _timeProvider = timeProvider;
            _options = options.Value;
        }

        public async Task<WeatherData> GetCurrentWeatherAsync(double lat, double lon, CancellationToken ct = default)
        {
            var cacheKey = BuildCacheKey("weather", lat, lon);

            if (TryGetFromCache<WeatherData>(cacheKey, out var cachedData) && cachedData is not null)
            {
                _logger.LogInformation("Возвращаем данные текущей погоды из кэша");
                return cachedData;
            }

            _logger.LogInformation("Кэш текущей погоды пуст, запрашиваем API");
            var data = await _apiClient.GetCurrentWeatherAsync(lat, lon, ct)
                ?? throw new InvalidOperationException(
                    $"API вернул null для координат {lat},{lon}");

            SetWithTtl(cacheKey, data, _options.CurrentWeatherCacheTtl);

            return data;
        }

        public async Task<ForecastData> GetForecastAsync(double lat, double lon, int dayCount, CancellationToken ct = default)
        {
            var cacheKey = BuildCacheKey("forecast", lat, lon, dayCount);

            if (TryGetFromCache<ForecastData>(cacheKey, out var cachedData) && cachedData is not null)
            {
                _logger.LogInformation("Возвращаем данные прогноза из кэша");
                return cachedData;
            }

            _logger.LogInformation("Кэш прогноза пуст, запрашиваем API");
            var data = await _apiClient.GetForecastAsync(lat, lon, dayCount, ct)
                ?? throw new InvalidOperationException(
                    $"API вернул null для координат {lat},{lon}, {dayCount}");

            SetWithTtl(cacheKey, data, _options.ForecastCacheTtl);

            return data;
        }

        private void SetWithTtl<T>(string key, T data, TimeSpan ttl) where T : class
        {
            var entry = new CacheEntry<T>(data, _timeProvider.GetUtcNow() + ttl);
            _cache.Set(key, entry, ttl);

        }

        private bool TryGetFromCache<T>(string key, out T? value) where T : class
        {
            if (_cache.TryGetValue(key, out CacheEntry<T>? entry) && entry is not null)
            {
                if (entry.ExpiresAt > _timeProvider.GetUtcNow())
                {
                    value = entry.Data;
                    return true;
                }

                // истекло — удаляем явно
                _cache.Remove(key);
            }

            value = null;
            return false;
        }

        private static string BuildCacheKey(string prefix, double lat, double lon, int? dayCount = null)
        {
            return dayCount is null
                ? string.Create(CultureInfo.InvariantCulture, $"{prefix}_{lat}_{lon}")
                : string.Create(CultureInfo.InvariantCulture, $"{prefix}_{lat}_{lon}_{dayCount}");
        }

        private sealed record CacheEntry<T>(T Data, DateTimeOffset ExpiresAt) where T : class;
    }
}
