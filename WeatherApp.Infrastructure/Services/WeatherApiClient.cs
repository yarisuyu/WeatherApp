using Microsoft.Extensions.Options;
using System.Globalization;
using System.Net.Http.Json;
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

        public async Task<WeatherData> GetCurrentWeatherAsync(double lat, double lon, CancellationToken ct = default)
        {
            // Запрос текущей погоды

            var latStr = lat.ToString(CultureInfo.InvariantCulture);
            var lonStr = lon.ToString(CultureInfo.InvariantCulture);
            var currentUrl = $"{_options.BaseUrl}/current.json?key={_options.ApiKey}&q={latStr},{lonStr}";
            var currentResponse = await _httpClient.GetFromJsonAsync<CurrentWeatherResponse>(currentUrl, ct)
                ?? throw new Exception("Failed to get current weather");

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

            return new WeatherData(location, currentWeather);
        }

        public async Task<ForecastData> GetForecastAsync(double lat, double lon, int dayCount, CancellationToken ct = default)
        {
            // Запрос прогноза на 3 дня
            var latStr = lat.ToString(CultureInfo.InvariantCulture);
            var lonStr = lon.ToString(CultureInfo.InvariantCulture);
            var forecastUrl = $"{_options.BaseUrl}/forecast.json?key={_options.ApiKey}&q={latStr},{lonStr}&days={dayCount}";
            var forecastResponse = await _httpClient.GetFromJsonAsync<ForecastResponse>(forecastUrl, ct)
                ?? throw new Exception("Failed to get forecast");

            // Преобразуем ответы в доменные сущности
            var location = new Location(
                forecastResponse.Location.Name,
                forecastResponse.Location.Lat,
                forecastResponse.Location.Lon);

            // Почасовой прогноз (все часы из forecast)
            var hourlyForecasts = forecastResponse.Forecast.ForecastDay
                .SelectMany(day => day.Hour)
                .Select(h => new HourlyForecast(
                    DateTime.Parse(h.Time),
                    Temperature.FromCelsius(h.TempC),
                    h.Condition.Code,
                    h.Condition.Text,
                    WindSpeed.FromKph(h.WindKph),
                    h.Humidity))
                .ToList();

            // Дневной прогноз (3 дня)
            var dailyForecasts = forecastResponse.Forecast.ForecastDay
                .Select(d => new DailyForecast(
                    DateTime.Parse(d.Date),
                    Temperature.FromCelsius(d.Day.MaxTempC),
                    Temperature.FromCelsius(d.Day.MinTempC),
                    Temperature.FromCelsius(d.Day.AvgTempC),
                    d.Day.Condition.Code,
                    d.Day.Condition.Text,
                    d.Day.TotalPrecipMm))
                .ToList();

            return new ForecastData(location, hourlyForecasts, dailyForecasts);
        }
    }
}
