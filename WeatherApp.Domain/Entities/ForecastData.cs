namespace WeatherApp.Domain.Entities
{
    public sealed class ForecastData
    {
        public Location Location { get; }
        public IReadOnlyList<HourlyForecast> HourlyForecasts { get; }
        public IReadOnlyList<DailyForecast> DailyForecasts { get; }

        public ForecastData(
            Location location,
            IReadOnlyList<HourlyForecast> hourlyForecasts,
            IReadOnlyList<DailyForecast> dailyForecasts)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            HourlyForecasts = hourlyForecasts ?? throw new ArgumentNullException(nameof(hourlyForecasts));
            DailyForecasts = dailyForecasts ?? throw new ArgumentNullException(nameof(dailyForecasts));
        }
    }
}
