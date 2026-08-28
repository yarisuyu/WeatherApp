using WeatherApp.Application.Interfaces;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Infrastructure.Services
{
    public class WeatherApiClient : IWeatherApiClient
    {        
        public Task<WeatherData> GetWeatherDataAsync(double lat, double lon, CancellationToken ct = default)
        {
            throw new Exception("NOT IMPLEMENTED!");
        }
    }
}
