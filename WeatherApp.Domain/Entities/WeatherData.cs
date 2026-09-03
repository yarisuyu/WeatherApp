namespace WeatherApp.Domain.Entities
{
   public sealed class WeatherData
    {
        public Location Location { get; }
        public CurrentWeather Current { get; }

        public WeatherData(
            Location location,
            CurrentWeather current)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Current = current ?? throw new ArgumentNullException(nameof(current));
        }
    }
}
