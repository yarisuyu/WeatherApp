using WeatherApp.Domain.Entities;
using WeatherApp.Domain.ValueObjects;

namespace WeatherApp.Application.UnitTests.TestData;

/// <summary>
/// Фабрика тестовых данных для тестов Application слоя.
/// Все методы возвращают валидные по умолчанию объекты, которые можно
/// переопределить через опциональные параметры.
/// </summary>
public static class WeatherTestDataFactory
{
    // Константы по умолчанию — координаты Москвы и типовые значения
    public const double DefaultLatitude = 55.7558;
    public const double DefaultLongitude = 37.6173;
    public const string DefaultCity = "Moscow";

    public static Location CreateLocation(
        string city = DefaultCity,
        double latitude = DefaultLatitude,
        double longitude = DefaultLongitude)
        => new(city, latitude, longitude);

    public static CurrentWeather CreateCurrentWeather(
        DateTime? lastUpdated = null,
        double temperatureCelsius = 22.5,
        int conditionCode = 1000,
        string conditionText = "Sunny",
        double windSpeedKph = 10.5,
        int humidityPercent = 65,
        double feelsLikeCelsius = 20.0,
        double uvIndex = 5.0)
        => new(
            lastUpdated ?? new DateTime(2026, 9, 10, 12, 0, 0, DateTimeKind.Utc),
            Temperature.FromCelsius(temperatureCelsius),
            conditionCode,
            conditionText,
            WindSpeed.FromKph(windSpeedKph),
            humidityPercent,
            Temperature.FromCelsius(feelsLikeCelsius),
            uvIndex);

    public static HourlyForecast CreateHourlyForecast(
        DateTime? time = null,
        double temperatureCelsius = 18.5,
        int conditionCode = 1000,
        string conditionText = "Sunny",
        double windSpeedKph = 8.0,
        int humidityPercent = 55)
        => new(
            time ?? new DateTime(2026, 9, 10, 14, 0, 0, DateTimeKind.Utc),
            Temperature.FromCelsius(temperatureCelsius),
            conditionCode,
            conditionText,
            WindSpeed.FromKph(windSpeedKph),
            humidityPercent);

    public static DailyForecast CreateDailyForecast(
        DateTime? date = null,
        double maxTemperatureCelsius = 25.0,
        double minTemperatureCelsius = 15.0,
        double avgTemperatureCelsius = 20.0,
        int conditionCode = 1000,
        string conditionText = "Sunny",
        double totalPrecipitationMm = 0.0)
        => new(
            date ?? new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc),
            Temperature.FromCelsius(maxTemperatureCelsius),
            Temperature.FromCelsius(minTemperatureCelsius),
            Temperature.FromCelsius(avgTemperatureCelsius),
            conditionCode,
            conditionText,
            totalPrecipitationMm);

    /// <summary>
    /// Создаёт список часовых прогнозов, начиная с указанного времени с шагом в 1 час.
    /// </summary>
    public static List<HourlyForecast> CreateHourlyForecasts(
        int count = 24,
        DateTime? startTime = null)
    {
        var start = startTime ?? new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc);
        return Enumerable.Range(0, count)
            .Select(i => CreateHourlyForecast(
                time: start.AddHours(i),
                temperatureCelsius: 15 + (i % 10))) // небольшое разнообразие
            .ToList();
    }

    /// <summary>
    /// Создаёт список дневных прогнозов, начиная с указанной даты.
    /// </summary>
    public static List<DailyForecast> CreateDailyForecasts(
        int count = 3,
        DateTime? startDate = null)
    {
        var start = startDate ?? new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc);
        return Enumerable.Range(0, count)
            .Select(i => CreateDailyForecast(
                date: start.AddDays(i),
                maxTemperatureCelsius: 25 - i,
                minTemperatureCelsius: 15 - i,
                avgTemperatureCelsius: 20 - i))
            .ToList();
    }

    /// <summary>
    /// Создаёт полный агрегат WeatherData с валидными значениями по умолчанию.
    /// </summary>
    public static WeatherData CreateWeatherData(
        Location? location = null,
        CurrentWeather? current = null)
        => new(
            location ?? CreateLocation(),
            current ?? CreateCurrentWeather());

    /// <summary>
    /// Создаёт полный агрегат ForecastData с валидными значениями по умолчанию.
    /// </summary>
    public static ForecastData CreateForecastData(
        Location? location = null,
        CurrentWeather? current = null,
        IReadOnlyList<HourlyForecast>? hourlyForecasts = null,
        IReadOnlyList<DailyForecast>? dailyForecasts = null)
        => new(
            location ?? CreateLocation(),
            hourlyForecasts ?? CreateHourlyForecasts(),
            dailyForecasts ?? CreateDailyForecasts());
}