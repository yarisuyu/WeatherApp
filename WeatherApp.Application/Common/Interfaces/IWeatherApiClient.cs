using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Common.Interfaces
{
    public interface IWeatherApiClient
    {
        Task<WeatherData> GetCurrentWeatherAsync(double lat, double lon, CancellationToken ct = default);
        Task<ForecastData> GetForecastAsync(double lat, double lon, int dayCount, CancellationToken ct = default);
    }
}
