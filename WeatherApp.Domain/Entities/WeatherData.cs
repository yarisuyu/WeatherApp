namespace WeatherApp.Domain.Entities
{
   public sealed class WeatherData
    {
        public Location Location { get; }
        public CurrentWeather Current { get; }
        public IReadOnlyList<HourlyForecast> HourlyForecasts { get; }
        public IReadOnlyList<DailyForecast> DailyForecasts { get; }

        public WeatherData(
            Location location,
            CurrentWeather current,
            IReadOnlyList<HourlyForecast> hourlyForecasts,
            IReadOnlyList<DailyForecast> dailyForecasts)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Current = current ?? throw new ArgumentNullException(nameof(current));
            HourlyForecasts = hourlyForecasts ?? throw new ArgumentNullException(nameof(hourlyForecasts));
            DailyForecasts = dailyForecasts ?? throw new ArgumentNullException(nameof(dailyForecasts));
        }
    }
}
