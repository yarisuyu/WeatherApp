using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.Interfaces
{
    public interface IWeatherApiClient
    {
        Task<WeatherData> GetWeatherDataAsync(double lat, double lon, CancellationToken ct = default);
    }
}
