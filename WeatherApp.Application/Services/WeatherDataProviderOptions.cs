namespace WeatherApp.Application.Services;

public class WeatherDataProviderOptions
{
    public const string SectionName = "WeatherDataProvider";

    public TimeSpan CurrentWeatherCacheTtl { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan ForecastCacheTtl { get; set; } = TimeSpan.FromMinutes(60);
}