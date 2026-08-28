using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherApp.Application.Common.Interfaces;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.ValueObjects;
using WeatherApp.Infrastructure.Models;
using WeatherApp.Infrastructure.Models.ApiResponses;
using WeatherApp.Infrastructure.Options;

namespace WeatherApp.Infrastructure.Services
{
    public class WeatherApiClient : IWeatherApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherApiOptions _options;

        public WeatherApiClient(HttpClient httpClient, IOptions<WeatherApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        public async Task<WeatherData> GetWeatherDataAsync(double lat, double lon, CancellationToken ct = default)
        {
            // Запрос текущей погоды
            var currentUrl = $"{_options.BaseUrl}/current.json?key={_options.ApiKey}&q={lat},{lon}";
            var currentResponse = await _httpClient.GetFromJsonAsync<CurrentWeatherResponse>(currentUrl, ct)
                ?? throw new Exception("Failed to get current weather");

            Console.Write(currentResponse);

            // Преобразуем ответы в доменные сущности
            var location = new Location(
                currentResponse.Location.Name,
                currentResponse.Location.Lat,
                currentResponse.Location.Lon);

            var currentWeather = new CurrentWeather(
                DateTime.Parse(currentResponse.Current.LastUpdated),
                Temperature.FromCelsius(currentResponse.Current.TempC),
                currentResponse.Current.Condition.Code,
                currentResponse.Current.Condition.Text,
                WindSpeed.FromKph(currentResponse.Current.WindKph),
                currentResponse.Current.Humidity,
                Temperature.FromCelsius(currentResponse.Current.FeelsLikeC),
                currentResponse.Current.Uv);

            return new WeatherData(location, currentWeather, [], []);
        }
    }
}
