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

        /// <summary>
        /// Возвращает оставшиеся часы текущего дня (начиная с указанного времени)
        /// и все часы следующего дня.
        /// </summary>
        public IEnumerable<HourlyForecast> GetRemainingHoursTodayAndTomorrow(DateTime now)
        {
            var today = now.Date;
            var tomorrow = today.AddDays(1);

            return HourlyForecasts
                .Where(h =>
                    (h.Time.Date == today && h.Time >= now) ||
                    h.Time.Date == tomorrow)
                .OrderBy(h => h.Time);
        }
    }
}
